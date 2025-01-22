using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.Expression;
namespace Ultra_Bend.Common.Math
{

    [DataContract(IsReference = false, Name = "RegressionCoefficient", Namespace = "http://www.ultradeep.com/")]
    public class RegressionCoefficient
    {

        public RegressionCoefficient(Expr expression, double scaling)
        {
            SetExpression(expression);
            Scaling = Structure.CollectIdentifierSymbols(_expression).ToDictionary(s => s.Item, s => scaling);
        }

        public RegressionCoefficient(Expr expression, Dictionary<string, double> scaling1, Dictionary<string, double> scaling2)
        {
            SetExpression(expression);
            Scaling = new Dictionary<string, double>(scaling1);
            foreach (var key in scaling2.Keys)
                Scaling[key] = scaling2[key];
        }

        [DataMember]
        public Dictionary<string, double> Scaling { get; set; }
        
        public double Coefficient => Value * EvalCoefficient();

        private string _expressionString { get; set; }

        [DataMember]
        public string ExpressionString
        {
            get => _expressionString;
            set => SetExpression(Infix.ParseOrThrow(value));
        }

        private Expr _expression { get; set; }

        [DataMember]
        public double Value { get; set; }

        public Expr GetExpression()
        {
            return _expression;
        }


        public void SetExpression(Expr expression)
        {
            _expression = expression;
            _expressionString = Infix.Format(expression);
            Scaling = Structure.CollectIdentifierSymbols(_expression).ToDictionary(s => s.Item, s => 1.0);
        }

        private double EvalCoefficient()
        {
            var X = new Dictionary<string, FloatingPoint>();
            foreach (var symbol in Structure.CollectIdentifierSymbols(_expression))
                X[symbol.Item] = 1.0 / Scaling[symbol.Item];
            return Evaluate.Evaluate(X, _expression).RealValue;
        }

        public double Eval(Dictionary<string, FloatingPoint> Xin)
        {
            var X = new Dictionary<string, FloatingPoint>(Xin);
            var keys = X.Keys.ToList();
            for (var i = 0; i < X.Keys.Count; i++)
                if (Scaling.ContainsKey(keys[i]))
                    X[keys[i]] = X[keys[i]].RealValue / Scaling[keys[i]];
                else
                    X[keys[i]] = X[keys[i]].RealValue;

            if (!Structure.CollectIdentifierSymbols(_expression).Any())
                return Evaluate.Evaluate(X, _expression).RealValue;

            return Evaluate.Evaluate(X, _expression).RealValue;
        }
    }
}
