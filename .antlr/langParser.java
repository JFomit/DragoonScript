// Generated from /home/jfomit/Uni/projects/semester-1/compiler/lang.g4 by ANTLR 4.9.2
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class langParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.9.2", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		NEW_LINE=1, WS=2, LEFT_PARENTHESIS=3, RIGHT_PARENTHESIS=4, MULTIPLY=5, 
		SIGNATURE_ARROW=6, LAMBDA_ARROW=7, COLON=8, FN=9, LET=10, TYPE=11, IS=12, 
		ABSURD=13, IDENTIFIER=14, UNIT=15, INTEGER=16, FLOAT=17, OPERATOR=18;
	public static final int
		RULE_file = 0, RULE_declaration = 1, RULE_type_declaration = 2, RULE_function_declaration = 3, 
		RULE_parameter_list = 4, RULE_parameter = 5, RULE_primary_expression = 6, 
		RULE_variable_reference = 7, RULE_literal = 8, RULE_let_binding = 9, RULE_pattern = 10, 
		RULE_type_expression = 11, RULE_simple_type = 12;
	private static String[] makeRuleNames() {
		return new String[] {
			"file", "declaration", "type_declaration", "function_declaration", "parameter_list", 
			"parameter", "primary_expression", "variable_reference", "literal", "let_binding", 
			"pattern", "type_expression", "simple_type"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "'\n'", null, "'('", "')'", "'*'", "'->'", "'=>'", "':'", "'fn'", 
			"'let'", "'type'", "'='", "'!'", null, "'()'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, "NEW_LINE", "WS", "LEFT_PARENTHESIS", "RIGHT_PARENTHESIS", "MULTIPLY", 
			"SIGNATURE_ARROW", "LAMBDA_ARROW", "COLON", "FN", "LET", "TYPE", "IS", 
			"ABSURD", "IDENTIFIER", "UNIT", "INTEGER", "FLOAT", "OPERATOR"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "lang.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public langParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	public static class FileContext extends ParserRuleContext {
		public TerminalNode EOF() { return getToken(langParser.EOF, 0); }
		public List<DeclarationContext> declaration() {
			return getRuleContexts(DeclarationContext.class);
		}
		public DeclarationContext declaration(int i) {
			return getRuleContext(DeclarationContext.class,i);
		}
		public FileContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_file; }
	}

	public final FileContext file() throws RecognitionException {
		FileContext _localctx = new FileContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_file);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(29);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << FN) | (1L << LET) | (1L << TYPE))) != 0)) {
				{
				{
				setState(26);
				declaration();
				}
				}
				setState(31);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			setState(32);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class DeclarationContext extends ParserRuleContext {
		public Type_declarationContext type_declaration() {
			return getRuleContext(Type_declarationContext.class,0);
		}
		public Function_declarationContext function_declaration() {
			return getRuleContext(Function_declarationContext.class,0);
		}
		public Let_bindingContext let_binding() {
			return getRuleContext(Let_bindingContext.class,0);
		}
		public DeclarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_declaration; }
	}

	public final DeclarationContext declaration() throws RecognitionException {
		DeclarationContext _localctx = new DeclarationContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_declaration);
		try {
			setState(37);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case TYPE:
				enterOuterAlt(_localctx, 1);
				{
				setState(34);
				type_declaration();
				}
				break;
			case FN:
				enterOuterAlt(_localctx, 2);
				{
				setState(35);
				function_declaration();
				}
				break;
			case LET:
				enterOuterAlt(_localctx, 3);
				{
				setState(36);
				let_binding();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_declarationContext extends ParserRuleContext {
		public Token name;
		public TerminalNode TYPE() { return getToken(langParser.TYPE, 0); }
		public TerminalNode IDENTIFIER() { return getToken(langParser.IDENTIFIER, 0); }
		public Type_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_declaration; }
	}

	public final Type_declarationContext type_declaration() throws RecognitionException {
		Type_declarationContext _localctx = new Type_declarationContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_type_declaration);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(39);
			match(TYPE);
			setState(40);
			((Type_declarationContext)_localctx).name = match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Function_declarationContext extends ParserRuleContext {
		public Token name;
		public Parameter_listContext params;
		public Type_expressionContext signature;
		public Primary_expressionContext body;
		public TerminalNode FN() { return getToken(langParser.FN, 0); }
		public TerminalNode IS() { return getToken(langParser.IS, 0); }
		public TerminalNode IDENTIFIER() { return getToken(langParser.IDENTIFIER, 0); }
		public Parameter_listContext parameter_list() {
			return getRuleContext(Parameter_listContext.class,0);
		}
		public Primary_expressionContext primary_expression() {
			return getRuleContext(Primary_expressionContext.class,0);
		}
		public TerminalNode COLON() { return getToken(langParser.COLON, 0); }
		public Type_expressionContext type_expression() {
			return getRuleContext(Type_expressionContext.class,0);
		}
		public Function_declarationContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_function_declaration; }
	}

	public final Function_declarationContext function_declaration() throws RecognitionException {
		Function_declarationContext _localctx = new Function_declarationContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_function_declaration);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(42);
			match(FN);
			setState(43);
			((Function_declarationContext)_localctx).name = match(IDENTIFIER);
			setState(44);
			((Function_declarationContext)_localctx).params = parameter_list();
			setState(47);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COLON) {
				{
				setState(45);
				match(COLON);
				setState(46);
				((Function_declarationContext)_localctx).signature = type_expression(0);
				}
			}

			setState(49);
			match(IS);
			setState(50);
			((Function_declarationContext)_localctx).body = primary_expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Parameter_listContext extends ParserRuleContext {
		public ParameterContext head;
		public Parameter_listContext tail;
		public ParameterContext parameter() {
			return getRuleContext(ParameterContext.class,0);
		}
		public Parameter_listContext parameter_list() {
			return getRuleContext(Parameter_listContext.class,0);
		}
		public Parameter_listContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter_list; }
	}

	public final Parameter_listContext parameter_list() throws RecognitionException {
		Parameter_listContext _localctx = new Parameter_listContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_parameter_list);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(52);
			((Parameter_listContext)_localctx).head = parameter();
			setState(54);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==IDENTIFIER) {
				{
				setState(53);
				((Parameter_listContext)_localctx).tail = parameter_list();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParameterContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(langParser.IDENTIFIER, 0); }
		public ParameterContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameter; }
	}

	public final ParameterContext parameter() throws RecognitionException {
		ParameterContext _localctx = new ParameterContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_parameter);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(56);
			match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Primary_expressionContext extends ParserRuleContext {
		public Primary_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_primary_expression; }
	 
		public Primary_expressionContext() { }
		public void copyFrom(Primary_expressionContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Parenthisised_expressionContext extends Primary_expressionContext {
		public TerminalNode LEFT_PARENTHESIS() { return getToken(langParser.LEFT_PARENTHESIS, 0); }
		public Primary_expressionContext primary_expression() {
			return getRuleContext(Primary_expressionContext.class,0);
		}
		public TerminalNode RIGHT_PARENTHESIS() { return getToken(langParser.RIGHT_PARENTHESIS, 0); }
		public Parenthisised_expressionContext(Primary_expressionContext ctx) { copyFrom(ctx); }
	}
	public static class Variable_expressionContext extends Primary_expressionContext {
		public Variable_referenceContext variable_reference() {
			return getRuleContext(Variable_referenceContext.class,0);
		}
		public Variable_expressionContext(Primary_expressionContext ctx) { copyFrom(ctx); }
	}
	public static class Literal_expressionContext extends Primary_expressionContext {
		public LiteralContext literal() {
			return getRuleContext(LiteralContext.class,0);
		}
		public Literal_expressionContext(Primary_expressionContext ctx) { copyFrom(ctx); }
	}

	public final Primary_expressionContext primary_expression() throws RecognitionException {
		Primary_expressionContext _localctx = new Primary_expressionContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_primary_expression);
		try {
			setState(64);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case LEFT_PARENTHESIS:
				_localctx = new Parenthisised_expressionContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(58);
				match(LEFT_PARENTHESIS);
				setState(59);
				primary_expression();
				setState(60);
				match(RIGHT_PARENTHESIS);
				}
				break;
			case INTEGER:
			case FLOAT:
				_localctx = new Literal_expressionContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(62);
				literal();
				}
				break;
			case IDENTIFIER:
				_localctx = new Variable_expressionContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(63);
				variable_reference();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Variable_referenceContext extends ParserRuleContext {
		public Token name;
		public TerminalNode IDENTIFIER() { return getToken(langParser.IDENTIFIER, 0); }
		public Variable_referenceContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variable_reference; }
	}

	public final Variable_referenceContext variable_reference() throws RecognitionException {
		Variable_referenceContext _localctx = new Variable_referenceContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_variable_reference);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(66);
			((Variable_referenceContext)_localctx).name = match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LiteralContext extends ParserRuleContext {
		public LiteralContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_literal; }
	 
		public LiteralContext() { }
		public void copyFrom(LiteralContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class FloatContext extends LiteralContext {
		public TerminalNode FLOAT() { return getToken(langParser.FLOAT, 0); }
		public FloatContext(LiteralContext ctx) { copyFrom(ctx); }
	}
	public static class IntContext extends LiteralContext {
		public TerminalNode INTEGER() { return getToken(langParser.INTEGER, 0); }
		public IntContext(LiteralContext ctx) { copyFrom(ctx); }
	}

	public final LiteralContext literal() throws RecognitionException {
		LiteralContext _localctx = new LiteralContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_literal);
		try {
			setState(70);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case INTEGER:
				_localctx = new IntContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(68);
				match(INTEGER);
				}
				break;
			case FLOAT:
				_localctx = new FloatContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(69);
				match(FLOAT);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Let_bindingContext extends ParserRuleContext {
		public PatternContext binding_pattern;
		public Primary_expressionContext value;
		public TerminalNode LET() { return getToken(langParser.LET, 0); }
		public TerminalNode IS() { return getToken(langParser.IS, 0); }
		public PatternContext pattern() {
			return getRuleContext(PatternContext.class,0);
		}
		public Primary_expressionContext primary_expression() {
			return getRuleContext(Primary_expressionContext.class,0);
		}
		public Let_bindingContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_let_binding; }
	}

	public final Let_bindingContext let_binding() throws RecognitionException {
		Let_bindingContext _localctx = new Let_bindingContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_let_binding);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(72);
			match(LET);
			setState(73);
			((Let_bindingContext)_localctx).binding_pattern = pattern();
			setState(74);
			match(IS);
			setState(75);
			((Let_bindingContext)_localctx).value = primary_expression();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PatternContext extends ParserRuleContext {
		public PatternContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_pattern; }
	 
		public PatternContext() { }
		public void copyFrom(PatternContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class VariableContext extends PatternContext {
		public Token variable;
		public TerminalNode IDENTIFIER() { return getToken(langParser.IDENTIFIER, 0); }
		public VariableContext(PatternContext ctx) { copyFrom(ctx); }
	}

	public final PatternContext pattern() throws RecognitionException {
		PatternContext _localctx = new PatternContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_pattern);
		try {
			_localctx = new VariableContext(_localctx);
			enterOuterAlt(_localctx, 1);
			{
			setState(77);
			((VariableContext)_localctx).variable = match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class Type_expressionContext extends ParserRuleContext {
		public Type_expressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_type_expression; }
	 
		public Type_expressionContext() { }
		public void copyFrom(Type_expressionContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class Function_signature_typeContext extends Type_expressionContext {
		public List<Type_expressionContext> type_expression() {
			return getRuleContexts(Type_expressionContext.class);
		}
		public Type_expressionContext type_expression(int i) {
			return getRuleContext(Type_expressionContext.class,i);
		}
		public TerminalNode SIGNATURE_ARROW() { return getToken(langParser.SIGNATURE_ARROW, 0); }
		public Function_signature_typeContext(Type_expressionContext ctx) { copyFrom(ctx); }
	}
	public static class Parenthisised_typeContext extends Type_expressionContext {
		public TerminalNode LEFT_PARENTHESIS() { return getToken(langParser.LEFT_PARENTHESIS, 0); }
		public Type_expressionContext type_expression() {
			return getRuleContext(Type_expressionContext.class,0);
		}
		public TerminalNode RIGHT_PARENTHESIS() { return getToken(langParser.RIGHT_PARENTHESIS, 0); }
		public Parenthisised_typeContext(Type_expressionContext ctx) { copyFrom(ctx); }
	}
	public static class Primitive_typeContext extends Type_expressionContext {
		public Simple_typeContext simple_type() {
			return getRuleContext(Simple_typeContext.class,0);
		}
		public Primitive_typeContext(Type_expressionContext ctx) { copyFrom(ctx); }
	}
	public static class Tuple_typeContext extends Type_expressionContext {
		public List<Type_expressionContext> type_expression() {
			return getRuleContexts(Type_expressionContext.class);
		}
		public Type_expressionContext type_expression(int i) {
			return getRuleContext(Type_expressionContext.class,i);
		}
		public TerminalNode MULTIPLY() { return getToken(langParser.MULTIPLY, 0); }
		public Tuple_typeContext(Type_expressionContext ctx) { copyFrom(ctx); }
	}

	public final Type_expressionContext type_expression() throws RecognitionException {
		return type_expression(0);
	}

	private Type_expressionContext type_expression(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		Type_expressionContext _localctx = new Type_expressionContext(_ctx, _parentState);
		Type_expressionContext _prevctx = _localctx;
		int _startState = 22;
		enterRecursionRule(_localctx, 22, RULE_type_expression, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(85);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case LEFT_PARENTHESIS:
				{
				_localctx = new Parenthisised_typeContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				setState(80);
				match(LEFT_PARENTHESIS);
				setState(81);
				type_expression(0);
				setState(82);
				match(RIGHT_PARENTHESIS);
				}
				break;
			case IDENTIFIER:
				{
				_localctx = new Primitive_typeContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(84);
				simple_type();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.LT(-1);
			setState(95);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,8,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(93);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,7,_ctx) ) {
					case 1:
						{
						_localctx = new Function_signature_typeContext(new Type_expressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_type_expression);
						setState(87);
						if (!(precpred(_ctx, 3))) throw new FailedPredicateException(this, "precpred(_ctx, 3)");
						setState(88);
						match(SIGNATURE_ARROW);
						setState(89);
						type_expression(4);
						}
						break;
					case 2:
						{
						_localctx = new Tuple_typeContext(new Type_expressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_type_expression);
						setState(90);
						if (!(precpred(_ctx, 2))) throw new FailedPredicateException(this, "precpred(_ctx, 2)");
						setState(91);
						match(MULTIPLY);
						setState(92);
						type_expression(3);
						}
						break;
					}
					} 
				}
				setState(97);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,8,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class Simple_typeContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(langParser.IDENTIFIER, 0); }
		public Simple_typeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_simple_type; }
	}

	public final Simple_typeContext simple_type() throws RecognitionException {
		Simple_typeContext _localctx = new Simple_typeContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_simple_type);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(98);
			match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 11:
			return type_expression_sempred((Type_expressionContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean type_expression_sempred(Type_expressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 3);
		case 1:
			return precpred(_ctx, 2);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\3\24g\4\2\t\2\4\3\t"+
		"\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t\13\4"+
		"\f\t\f\4\r\t\r\4\16\t\16\3\2\7\2\36\n\2\f\2\16\2!\13\2\3\2\3\2\3\3\3\3"+
		"\3\3\5\3(\n\3\3\4\3\4\3\4\3\5\3\5\3\5\3\5\3\5\5\5\62\n\5\3\5\3\5\3\5\3"+
		"\6\3\6\5\69\n\6\3\7\3\7\3\b\3\b\3\b\3\b\3\b\3\b\5\bC\n\b\3\t\3\t\3\n\3"+
		"\n\5\nI\n\n\3\13\3\13\3\13\3\13\3\13\3\f\3\f\3\r\3\r\3\r\3\r\3\r\3\r\5"+
		"\rX\n\r\3\r\3\r\3\r\3\r\3\r\3\r\7\r`\n\r\f\r\16\rc\13\r\3\16\3\16\3\16"+
		"\2\3\30\17\2\4\6\b\n\f\16\20\22\24\26\30\32\2\2\2d\2\37\3\2\2\2\4\'\3"+
		"\2\2\2\6)\3\2\2\2\b,\3\2\2\2\n\66\3\2\2\2\f:\3\2\2\2\16B\3\2\2\2\20D\3"+
		"\2\2\2\22H\3\2\2\2\24J\3\2\2\2\26O\3\2\2\2\30W\3\2\2\2\32d\3\2\2\2\34"+
		"\36\5\4\3\2\35\34\3\2\2\2\36!\3\2\2\2\37\35\3\2\2\2\37 \3\2\2\2 \"\3\2"+
		"\2\2!\37\3\2\2\2\"#\7\2\2\3#\3\3\2\2\2$(\5\6\4\2%(\5\b\5\2&(\5\24\13\2"+
		"\'$\3\2\2\2\'%\3\2\2\2\'&\3\2\2\2(\5\3\2\2\2)*\7\r\2\2*+\7\20\2\2+\7\3"+
		"\2\2\2,-\7\13\2\2-.\7\20\2\2.\61\5\n\6\2/\60\7\n\2\2\60\62\5\30\r\2\61"+
		"/\3\2\2\2\61\62\3\2\2\2\62\63\3\2\2\2\63\64\7\16\2\2\64\65\5\16\b\2\65"+
		"\t\3\2\2\2\668\5\f\7\2\679\5\n\6\28\67\3\2\2\289\3\2\2\29\13\3\2\2\2:"+
		";\7\20\2\2;\r\3\2\2\2<=\7\5\2\2=>\5\16\b\2>?\7\6\2\2?C\3\2\2\2@C\5\22"+
		"\n\2AC\5\20\t\2B<\3\2\2\2B@\3\2\2\2BA\3\2\2\2C\17\3\2\2\2DE\7\20\2\2E"+
		"\21\3\2\2\2FI\7\22\2\2GI\7\23\2\2HF\3\2\2\2HG\3\2\2\2I\23\3\2\2\2JK\7"+
		"\f\2\2KL\5\26\f\2LM\7\16\2\2MN\5\16\b\2N\25\3\2\2\2OP\7\20\2\2P\27\3\2"+
		"\2\2QR\b\r\1\2RS\7\5\2\2ST\5\30\r\2TU\7\6\2\2UX\3\2\2\2VX\5\32\16\2WQ"+
		"\3\2\2\2WV\3\2\2\2Xa\3\2\2\2YZ\f\5\2\2Z[\7\b\2\2[`\5\30\r\6\\]\f\4\2\2"+
		"]^\7\7\2\2^`\5\30\r\5_Y\3\2\2\2_\\\3\2\2\2`c\3\2\2\2a_\3\2\2\2ab\3\2\2"+
		"\2b\31\3\2\2\2ca\3\2\2\2de\7\20\2\2e\33\3\2\2\2\13\37\'\618BHW_a";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}