﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LispEngine.Lexing
{
    public class Scanner
    {
        private readonly TextReader input;
        private StringBuilder sb;

        public static Scanner Create(string s)
        {
            return new Scanner(new StringReader(s));
        }

        public Scanner(TextReader input)
        {
            this.input = input;
        }

        private char peek()
        {
            return (char) input.Peek();
        }

        private void readChar()
        {
            sb.Append((char) input.Read());
        }

        private bool more()
        {
            return input.Peek() != -1;
        }

        private Token tok(TokenType type)
        {
            return sb.Length > 0 ? new Token(type, sb.ToString()) : null;
        }

        private bool isLetter()
        {
            return more() && char.IsLetter(peek());
        }

        private bool isLetterOrDigit()
        {
            return more() && char.IsLetterOrDigit(peek());
        }

        private bool isDigit()
        {
            return more() && char.IsDigit(peek());
        }

        private bool isWhiteSpace()
        {
            return more() && char.IsWhiteSpace(peek());
        }

        private Token symbol()
        {
            if(!isLetter())
                return null;
            readChar();
            while(isLetterOrDigit())
                readChar();
            return tok(TokenType.Symbol);
        }

        private Token space()
        {
            while(isWhiteSpace())
                readChar();
            return tok(TokenType.Space);
        }

        private Token integer()
        {
            while (isDigit())
                readChar();
            
            return tok(TokenType.Integer);
        }

        private Token openClose()
        {
            if (peek() == '(')
            {
                readChar();
                return tok(TokenType.Open);
            }
            if (peek() == ')')
            {
                readChar();
                return tok(TokenType.Close);
            }
            return null;
        }

        public Token GetNext()
        {
            Token t;
            sb = new StringBuilder();
            if (!more())
                return null;
            if ((t = symbol()) != null)
                return t;
            if ((t = space()) != null)
                return t;
            if ((t = integer()) != null)
                return t;
            if ((t = openClose()) != null)
                return t;
            return null;
        }

        public IEnumerable<Token> Scan()
        {
            Token next;
            while ((next = GetNext()) != null)
                yield return next;
        }
    }
}