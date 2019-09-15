using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Ink.Runtime
{
    internal class Container : Object, INamedContent
    {
        public string name { get; set; }

        public List<Object> content { get { return _content; } set { AddContent(value); } }
        List<Object> _content;

        public Dictionary<string, INamedContent> namedContent { get; set; }

        public Dictionary<string, Object> namedOnlyContent
        {
            get
            {
                var namedOnlyContentDict = new Dictionary<string, Object>();
                foreach (var kvPair in namedContent)
                {
                    namedOnlyContentDict[kvPair.Key] = (Object) kvPair.Value;
                }

                foreach (var c in content)
                {
                    var named = c as INamedContent;
                    if (named != null && named.hasValidName)
                    {
                        namedOnlyContentDict.Remove(named.name);
                    }
                }

                if (namedOnlyContentDict.Count == 0)
                    namedOnlyContentDict = null;

                return namedOnlyContentDict;
            }
            set
            {
                var existingNamedOnly = namedOnlyContent;
                if (existingNamedOnly != null)
                {
                    foreach (var kvPair in existingNamedOnly)
                    {
                        namedContent.Remove(kvPair.Key);
                    }
                }

                if (value == null)
                    return;

                foreach (var kvPair in value)
                {
                    var named = kvPair.Value as INamedContent;
                    if (named != null)
                        AddToNamedContentOnly(named);
                }
            }
        }

        public bool visitsShouldBeCounted { get; set; }
        public bool turnIndexShouldBeCounted { get; set; }
        public bool countingAtStartOnly { get; set; }

        [Flags]
        public enum CountFlags
        {
            Visits = 1,
            Turns = 2,
            CountStartOnly = 4
        }

        public int countFlags
        {
            get
            {
                CountFlags flags = 0;
                if (visitsShouldBeCounted) flags |= CountFlags.Visits;
                if (turnIndexShouldBeCounted) flags |= CountFlags.Turns;
                if (countingAtStartOnly) flags |= CountFlags.CountStartOnly;

                // If we're only storing CountStartOnly, it serves no purpose,
                // since it's dependent on the other two to be used at all.
                // (e.g. for setting the fact that *if* a gather or choice's
                // content is counted, then is should only be counter at the start)
                // So this is just an optimisation for storage.
                if (flags == CountFlags.CountStartOnly)
                {
                    flags = 0;
                }

                return (int) flags;
            }
            set
            {
                var flag = (CountFlags) value;
                if ((flag & CountFlags.Visits) > 0) visitsShouldBeCounted = true;
                if ((flag & CountFlags.Turns) > 0) turnIndexShouldBeCounted = true;
                if ((flag & CountFlags.CountStartOnly) > 0) countingAtStartOnly = true;
            }
        }

        public bool hasValidName { get { return name != null && name.Length > 0; } }

        public Path pathToFirstLeafContent
        {
            get
            {
                if (_pathToFirstLeafContent == null)
                    _pathToFirstLeafContent = path.PathByAppendingPath(internalPathToFirstLeafContent);

                return _pathToFirstLeafContent;
            }
        }

        Path _pathToFirstLeafContent;

        Path internalPathToFirstLeafContent
        {
            get
            {
                var components = new List<Path.Component>();
                var container = this;
                while (container != null)
                {
                    if (container.content.Count > 0)
                    {
                        components.Add(new Path.Component(0));
                        container = container.content[0] as Container;
                    }
                }

                return new Path(components);
            }
        }

        public Container()
        {
            _content = new List<Object>();
            namedContent = new Dictionary<string, INamedContent>();
        }

        public void AddContent(Object contentObj)
        {
            content.Add(contentObj);

            if (contentObj.parent)
            {
                throw new Exception("content is already in " + contentObj.parent);
            }

            contentObj.parent = this;

            TryAddNamedContent(contentObj);
        }

        public void AddContent(IList<Object> contentList)
        {
            foreach (var c in contentList)
            {
                AddContent(c);
            }
        }

        public void InsertContent(Object contentObj, int index)
        {
            content.Insert(index, contentObj);

            if (contentObj.parent)
            {
                throw new Exception("content is already in " + contentObj.parent);
            }

            contentObj.parent = this;

            TryAddNamedContent(contentObj);
        }

        public void TryAddNamedContent(Object contentObj)
        {
            var namedContentObj = contentObj as INamedContent;
            if (namedContentObj != null && namedContentObj.hasValidName)
            {
                AddToNamedContentOnly(namedContentObj);
            }
        }

        public void AddToNamedContentOnly(INamedContent namedContentObj)
        {
            Debug.Assert(namedContentObj is Object, "Can only add Runtime.Objects to a Runtime.Container");
            var runtimeObj = (Object) namedContentObj;
            runtimeObj.parent = this;

            namedContent[namedContentObj.name] = namedContentObj;
        }

        public void AddContentsOfContainer(Container otherContainer)
        {
            content.AddRange(otherContainer.content);
            foreach (var obj in otherContainer.content)
            {
                obj.parent = this;
                TryAddNamedContent(obj);
            }
        }

        protected Object ContentWithPathComponent(Path.Component component)
        {
            if (component.isIndex)
            {
                if (component.index >= 0 && component.index < content.Count)
                {
                    return content[component.index];
                }

                // When path is out of range, quietly return nil
                // (useful as we step/increment forwards through content)
                else
                {
                    return null;
                }
            }

            else if (component.isParent)
            {
                return this.parent;
            }

            else
            {
                INamedContent foundContent = null;
                if (namedContent.TryGetValue(component.name, out foundContent))
                {
                    return (Object) foundContent;
                }
                else
                {
                    return null;
                }
            }
        }

        public SearchResult ContentAtPath(Path path, int partialPathStart = 0, int partialPathLength = -1)
        {
            if (partialPathLength == -1)
                partialPathLength = path.length;

            var result = new SearchResult();
            result.approximate = false;

            Container currentContainer = this;
            Object currentObj = this;

            for (int i = partialPathStart; i < partialPathLength; ++i)
            {
                var comp = path.GetComponent(i);

                // Path component was wrong type
                if (currentContainer == null)
                {
                    result.approximate = true;
                    break;
                }

                var foundObj = currentContainer.ContentWithPathComponent(comp);

                // Couldn't resolve entire path?
                if (foundObj == null)
                {
                    result.approximate = true;
                    break;
                }

                currentObj = foundObj;
                currentContainer = foundObj as Container;
            }

            result.obj = currentObj;

            return result;
        }

        public void BuildStringOfHierarchy(StringBuilder sb, int indentation, Object pointedObj)
        {
            Action appendIndentation = () =>
                                       {
                                           const int spacesPerIndent = 4;
                                           for (int i = 0; i < spacesPerIndent * indentation; ++i)
                                           {
                                               sb.Append(" ");
                                           }
                                       };

            appendIndentation();
            sb.Append("[");

            if (this.hasValidName)
            {
                sb.AppendFormat(" ({0})", this.name);
            }

            if (this == pointedObj)
            {
                sb.Append("  <---");
            }

            sb.AppendLine();

            indentation++;

            for (int i = 0; i < content.Count; ++i)
            {
                var obj = content[i];

                if (obj is Container)
                {
                    var container = (Container) obj;

                    container.BuildStringOfHierarchy(sb, indentation, pointedObj);
                }
                else
                {
                    appendIndentation();
                    if (obj is StringValue)
                    {
                        sb.Append("\"");
                        sb.Append(obj.ToString().Replace("\n", "\\n"));
                        sb.Append("\"");
                    }
                    else
                    {
                        sb.Append(obj.ToString());
                    }
                }

                if (i != content.Count - 1)
                {
                    sb.Append(",");
                }

                if (!(obj is Container) && obj == pointedObj)
                {
                    sb.Append("  <---");
                }

                sb.AppendLine();
            }


            var onlyNamed = new Dictionary<string, INamedContent>();

            foreach (var objKV in namedContent)
            {
                if (content.Contains((Object) objKV.Value))
                {
                    continue;
                }
                else
                {
                    onlyNamed.Add(objKV.Key, objKV.Value);
                }
            }

            if (onlyNamed.Count > 0)
            {
                appendIndentation();
                sb.AppendLine("-- named: --");

                foreach (var objKV in onlyNamed)
                {
                    Debug.Assert(objKV.Value is Container, "Can only print out named Containers");
                    var container = (Container) objKV.Value;
                    container.BuildStringOfHierarchy(sb, indentation, pointedObj);

                    sb.AppendLine();
                }
            }


            indentation--;

            appendIndentation();
            sb.Append("]");
        }

        public virtual string BuildStringOfHierarchy()
        {
            var sb = new StringBuilder();

            BuildStringOfHierarchy(sb, 0, null);

            return sb.ToString();
        }
    }
}