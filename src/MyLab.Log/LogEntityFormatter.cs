using System;
using System.Linq;
using MyLab.Log.Serializing;

namespace MyLab.Log
{
    /// <summary>
    /// Converts <see cref="LogEntity"/> to string
    /// </summary>
    public static class LogEntityFormatter
    {
        /// <summary>
        /// Yaml formatter function
        /// </summary>
        public static readonly Func<LogEntity, Exception, string> Yaml = (entity, exception) =>
        {
            try
            {
                return entity.ToYaml();
            }
            catch (Exception e)
            {
                FillLogException(e, entity, exception);

                try
                {
                    var json = entity.ToJson();

                    e.AndFactIs("Log entity json", json);
                }
                catch (Exception addParsingEx)
                {
                    e.AndFactIs("Json parsing error", ExceptionDto.Create(addParsingEx));
                }

                throw;
            }
        };

        /// <summary>
        /// Json formatter function
        /// </summary>
        public static readonly Func<LogEntity, Exception, string> Json = (entity, exception) =>
        {
            try
            {
                return entity.ToJson();
            }
            catch (Exception e)
            {
                FillLogException(e, entity, exception);

                try
                {
                    var json = entity.ToYaml();

                    e.AndFactIs("Log entity yaml", json);
                }
                catch (Exception addParsingEx)
                {
                    e.AndFactIs("Yaml parsing error", ExceptionDto.Create(addParsingEx));
                }

                throw;
            }
        };

        static void FillLogException(Exception actualException, object entity, Exception initialException)
        {
            if (entity == null)
            {
                actualException.AndLabel("no-entity");
            }
            else
            {
                actualException.AndFactIs("Entity type", entity.GetType().FullName);

                if (entity is LogEntity le)
                {
                    if (le.Labels != null && le.Labels.Count != 0)
                    {
                        actualException.AndFactIs("Labels",
                            string.Join(", ", le.Labels.Select(l => $"{l.Key}={l.Value}")));
                    }
                    else
                    {
                        actualException.AndLabel("no-labels");
                    }

                    if (le.Message != null)
                    {
                        actualException.AndFactIs("Log message", le.Message);
                    }
                    else
                    {
                        actualException.AndLabel("no-message");
                    }

                    if (le.Facts != null && le.Facts.Count != 0)
                    {
                        actualException.AndFactIs("Facts", le.Facts.Select(f => $"{f.Key}={f.Value}"));
                    }
                    else
                    {
                        actualException.AndLabel("no-facts");
                    }
                }
            }

            if (initialException == null)
            {
                actualException.AndLabel("no-exception");
            }
            else
            {
                actualException
                    .AndFactIs("Exception type", initialException.GetType().FullName)
                    .AndFactIs("Exception message", initialException.Message);
            }
        }
    }
}