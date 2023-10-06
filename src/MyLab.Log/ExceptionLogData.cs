using System;
using System.Collections;
using System.Collections.Generic;

namespace MyLab.Log
{

    /// <summary>
    /// Provides log tolls for an <see cref="Exception"/>
    /// </summary>
    public class ExceptionLogData
    {
        private const string FactsKey = "facts";
        private const string LabelsKey = "labels";
        private const string DataFactsKey = "data-entries";

        private readonly Exception _e;

        /// <summary>
        /// Initializes a new instance of <see cref="ExceptionLogData"/>
        /// </summary>
        public ExceptionLogData(Exception e)
        {
            _e = e ?? throw new ArgumentNullException(nameof(e));
        }

        /// <summary>
        /// Adds log fact into exception 
        /// </summary>
        public void AddFact(string factName, object factValue)
        {
            LogFacts exceptionFacts;
            if (_e.Data.Contains(FactsKey))
            {
                exceptionFacts = (LogFacts)_e.Data[FactsKey];
            }
            else
            {
                exceptionFacts = new LogFacts();
                _e.Data.Add(FactsKey, exceptionFacts);
            }

            if (exceptionFacts.ContainsKey(factName))
            {
                exceptionFacts[factName] = factValue;
            }
            else
            {
                exceptionFacts.Add(factName, factValue);
            }
        }

        /// <summary>
        /// Adds log label into exception
        /// </summary>
        public void AddLabel(string labelName, string labelValue)
        {
            LogLabels stringList;
            if (_e.Data.Contains(LabelsKey))
            {
                stringList = (LogLabels)_e.Data[LabelsKey];
            }
            else
            {
                stringList = new LogLabels();
                _e.Data.Add(LabelsKey, stringList);
            }

            if (stringList.ContainsKey(labelName))
            {
                stringList[labelName] = labelValue;
            }
            else
            {
                stringList.Add(labelName, labelValue);
            }
        }

        /// <summary>Gets conditions for Exception</summary>
        public LogFacts GetFacts()
        {
            LogFacts facts = null;

            var dataEntries = new Dictionary<string, object>();

            foreach (DictionaryEntry entry in _e.Data)
            {
                if (entry.Key is string strKey)
                {
                    if (strKey == FactsKey && entry.Value is LogFacts entryFacts)
                    {
                        facts = new LogFacts(entryFacts);
                    }
                    else
                    {
                        dataEntries.Add(strKey, entry.Value);
                    }
                }    
            }

            if (facts == null) facts = new LogFacts();

            if (dataEntries.Count > 0)
            {
                facts.Add(DataFactsKey, dataEntries);
            }

            return facts;
        }

        /// <summary>Gets label for Exception</summary>
        public LogLabels GetLabels()
        {
            if (_e.Data.Contains(LabelsKey))
                return (LogLabels)_e.Data[LabelsKey];
            return new LogLabels();
        }
    }
}
