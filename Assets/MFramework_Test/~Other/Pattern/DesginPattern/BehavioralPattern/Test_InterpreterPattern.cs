using MFramework;
using UnityEngine;

public class Test_InterpreterPattern : MonoBehaviour
{
    private void Start()
    {
        OrExpression orExpr = new OrExpression(new TerminalExpression("Robert"), new TerminalExpression("John"));
        //表达式中，具有两个数据Robert/John，只要传入数据中包含其中一个的字符串，即可判断成功
        bool b1 = orExpr.Interpret("Robert");
        MLog.Print(b1);

        AndExpression andExpr = new AndExpression(new TerminalExpression("Julie"), new TerminalExpression("Married"));
        //表达式中，具有两个数据Married/Julie，传入数据中必须同时包含两者字符串，才能判断成功
        bool b2 = andExpr.Interpret("Married Julie 123 HH");
        MLog.Print(b2);
    }

    public interface Expression
    {
        public bool Interpret(string context);
    }
    //终结符
    public class TerminalExpression : Expression
    {
        private string data;

        public TerminalExpression(string data)
        {
            this.data = data;
        }

        public bool Interpret(string context)
        {
            if (context.Contains(data))
            {
                return true;
            }
            return false;
        }
    }
    //非终结符---用于最终判断
    public class OrExpression : Expression
    {
        private Expression expr1 = null;
        private Expression expr2 = null;

        public OrExpression(Expression expr1, Expression expr2)
        {
            this.expr1 = expr1;
            this.expr2 = expr2;
        }

        public bool Interpret(string context)
        {
            return expr1.Interpret(context) || expr2.Interpret(context);
        }
    }
    public class AndExpression : Expression
    {
        private Expression expr1 = null;
        private Expression expr2 = null;

        public AndExpression(Expression expr1, Expression expr2)
        {
            this.expr1 = expr1;
            this.expr2 = expr2;
        }

        public bool Interpret(string context)
        {
            return expr1.Interpret(context) && expr2.Interpret(context);
        }
    }
}
