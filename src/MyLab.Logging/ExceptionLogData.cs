using System;

namespace MyLab.Logging
{

    public class ExceptionLogData
    {
        private const string FactsKey = "facts";
        private const string LabelsKey = "labels";

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
            exceptionFacts.Add(factName, factValue);
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
            stringList.Add(labelName, labelValue);
        }

        /// <summary>Gets conditions for Exception</summary>
        public LogFacts GetFacts()
        {
            if (_e.Data.Contains(FactsKey))
                return (LogFacts)_e.Data[FactsKey];
            return new LogFacts();
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
