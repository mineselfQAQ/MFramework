using MFramework;
using UnityEngine;

public class Test_InterpreterPattern : MonoBehaviour
{
    private void Start()
    {
        OrExpression orExpr = new OrExpression(new TerminalExpression("Robert"), new TerminalExpression("John"));
        //���ʽ�У�������������Robert/John��ֻҪ���������а�������һ�����ַ����������жϳɹ�
        bool b1 = orExpr.Interpret("Robert");
        MLog.Print(b1);

        AndExpression andExpr = new AndExpression(new TerminalExpression("Julie"), new TerminalExpression("Married"));
        //���ʽ�У�������������Married/Julie�����������б���ͬʱ���������ַ����������жϳɹ�
        bool b2 = andExpr.Interpret("Married Julie 123 HH");
        MLog.Print(b2);
    }

    public interface Expression
    {
        public bool Interpret(string context);
    }
    //�ս��
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
    //���ս��---���������ж�
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
