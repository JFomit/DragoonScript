// Generated from /home/jfomit/Uni/projects/semester-1/compiler/lang.g4 by ANTLR 4.9.2
import org.antlr.v4.runtime.Lexer;
import org.antlr.v4.runtime.CharStream;
import org.antlr.v4.runtime.Token;
import org.antlr.v4.runtime.TokenStream;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.misc.*;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class langLexer extends Lexer {
	static { RuntimeMetaData.checkVersion("4.9.2", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		NEW_LINE=1, WS=2, LEFT_PARENTHESIS=3, RIGHT_PARENTHESIS=4, MULTIPLY=5, 
		SIGNATURE_ARROW=6, LAMBDA_ARROW=7, COLON=8, FN=9, LET=10, TYPE=11, IS=12, 
		ABSURD=13, IDENTIFIER=14, UNIT=15, INTEGER=16, FLOAT=17, OPERATOR=18;
	public static String[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static String[] modeNames = {
		"DEFAULT_MODE"
	};

	private static String[] makeRuleNames() {
		return new String[] {
			"NEW_LINE", "WS", "LEFT_PARENTHESIS", "RIGHT_PARENTHESIS", "MULTIPLY", 
			"SIGNATURE_ARROW", "LAMBDA_ARROW", "COLON", "FN", "LET", "TYPE", "IS", 
			"ABSURD", "IDENTIFIER", "UNIT", "INTEGER", "FLOAT", "GRAVE", "OPERATOR"
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


	public langLexer(CharStream input) {
		super(input);
		_interp = new LexerATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@Override
	public String getGrammarFileName() { return "lang.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public String[] getChannelNames() { return channelNames; }

	@Override
	public String[] getModeNames() { return modeNames; }

	@Override
	public ATN getATN() { return _ATN; }

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\2\24s\b\1\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\3\2\3\2\3\3\3\3\3\4\3\4\3\5\3\5\3\6\3\6\3\7\3\7\3"+
		"\7\3\b\3\b\3\b\3\t\3\t\3\n\3\n\3\n\3\13\3\13\3\13\3\13\3\f\3\f\3\f\3\f"+
		"\3\f\3\r\3\r\3\16\3\16\3\17\3\17\7\17N\n\17\f\17\16\17Q\13\17\3\20\3\20"+
		"\3\20\3\21\6\21W\n\21\r\21\16\21X\3\22\7\22\\\n\22\f\22\16\22_\13\22\3"+
		"\22\3\22\6\22c\n\22\r\22\16\22d\3\23\3\23\3\24\6\24j\n\24\r\24\16\24k"+
		"\3\24\3\24\3\24\3\24\5\24r\n\24\2\2\25\3\3\5\4\7\5\t\6\13\7\r\b\17\t\21"+
		"\n\23\13\25\f\27\r\31\16\33\17\35\20\37\21!\22#\23%\2\'\24\3\2\7\4\2\13"+
		"\13\"\"\5\2C\\aac|\6\2\62;C\\aac|\3\2\62;\n\2##%(,-\60\61>B^^``~\u0080"+
		"\2w\2\3\3\2\2\2\2\5\3\2\2\2\2\7\3\2\2\2\2\t\3\2\2\2\2\13\3\2\2\2\2\r\3"+
		"\2\2\2\2\17\3\2\2\2\2\21\3\2\2\2\2\23\3\2\2\2\2\25\3\2\2\2\2\27\3\2\2"+
		"\2\2\31\3\2\2\2\2\33\3\2\2\2\2\35\3\2\2\2\2\37\3\2\2\2\2!\3\2\2\2\2#\3"+
		"\2\2\2\2\'\3\2\2\2\3)\3\2\2\2\5+\3\2\2\2\7-\3\2\2\2\t/\3\2\2\2\13\61\3"+
		"\2\2\2\r\63\3\2\2\2\17\66\3\2\2\2\219\3\2\2\2\23;\3\2\2\2\25>\3\2\2\2"+
		"\27B\3\2\2\2\31G\3\2\2\2\33I\3\2\2\2\35K\3\2\2\2\37R\3\2\2\2!V\3\2\2\2"+
		"#]\3\2\2\2%f\3\2\2\2\'q\3\2\2\2)*\7\f\2\2*\4\3\2\2\2+,\t\2\2\2,\6\3\2"+
		"\2\2-.\7*\2\2.\b\3\2\2\2/\60\7+\2\2\60\n\3\2\2\2\61\62\7,\2\2\62\f\3\2"+
		"\2\2\63\64\7/\2\2\64\65\7@\2\2\65\16\3\2\2\2\66\67\7?\2\2\678\7@\2\28"+
		"\20\3\2\2\29:\7<\2\2:\22\3\2\2\2;<\7h\2\2<=\7p\2\2=\24\3\2\2\2>?\7n\2"+
		"\2?@\7g\2\2@A\7v\2\2A\26\3\2\2\2BC\7v\2\2CD\7{\2\2DE\7r\2\2EF\7g\2\2F"+
		"\30\3\2\2\2GH\7?\2\2H\32\3\2\2\2IJ\7#\2\2J\34\3\2\2\2KO\t\3\2\2LN\t\4"+
		"\2\2ML\3\2\2\2NQ\3\2\2\2OM\3\2\2\2OP\3\2\2\2P\36\3\2\2\2QO\3\2\2\2RS\7"+
		"*\2\2ST\7+\2\2T \3\2\2\2UW\t\5\2\2VU\3\2\2\2WX\3\2\2\2XV\3\2\2\2XY\3\2"+
		"\2\2Y\"\3\2\2\2Z\\\t\5\2\2[Z\3\2\2\2\\_\3\2\2\2][\3\2\2\2]^\3\2\2\2^`"+
		"\3\2\2\2_]\3\2\2\2`b\7\60\2\2ac\t\5\2\2ba\3\2\2\2cd\3\2\2\2db\3\2\2\2"+
		"de\3\2\2\2e$\3\2\2\2fg\7b\2\2g&\3\2\2\2hj\t\6\2\2ih\3\2\2\2jk\3\2\2\2"+
		"ki\3\2\2\2kl\3\2\2\2lr\3\2\2\2mn\5%\23\2no\5\35\17\2op\5%\23\2pr\3\2\2"+
		"\2qi\3\2\2\2qm\3\2\2\2r(\3\2\2\2\t\2OX]dkq\2";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}