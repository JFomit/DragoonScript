// Generated from /home/jfomit/Uni/projects/semester-1/compiler/lang.g4 by ANTLR 4.9.2
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link langParser}.
 */
public interface langListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link langParser#file}.
	 * @param ctx the parse tree
	 */
	void enterFile(langParser.FileContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#file}.
	 * @param ctx the parse tree
	 */
	void exitFile(langParser.FileContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#declaration}.
	 * @param ctx the parse tree
	 */
	void enterDeclaration(langParser.DeclarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#declaration}.
	 * @param ctx the parse tree
	 */
	void exitDeclaration(langParser.DeclarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#type_declaration}.
	 * @param ctx the parse tree
	 */
	void enterType_declaration(langParser.Type_declarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#type_declaration}.
	 * @param ctx the parse tree
	 */
	void exitType_declaration(langParser.Type_declarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#function_declaration}.
	 * @param ctx the parse tree
	 */
	void enterFunction_declaration(langParser.Function_declarationContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#function_declaration}.
	 * @param ctx the parse tree
	 */
	void exitFunction_declaration(langParser.Function_declarationContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#parameter_list}.
	 * @param ctx the parse tree
	 */
	void enterParameter_list(langParser.Parameter_listContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#parameter_list}.
	 * @param ctx the parse tree
	 */
	void exitParameter_list(langParser.Parameter_listContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#parameter}.
	 * @param ctx the parse tree
	 */
	void enterParameter(langParser.ParameterContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#parameter}.
	 * @param ctx the parse tree
	 */
	void exitParameter(langParser.ParameterContext ctx);
	/**
	 * Enter a parse tree produced by the {@code parenthisised_expression}
	 * labeled alternative in {@link langParser#primary_expression}.
	 * @param ctx the parse tree
	 */
	void enterParenthisised_expression(langParser.Parenthisised_expressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code parenthisised_expression}
	 * labeled alternative in {@link langParser#primary_expression}.
	 * @param ctx the parse tree
	 */
	void exitParenthisised_expression(langParser.Parenthisised_expressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code literal_expression}
	 * labeled alternative in {@link langParser#primary_expression}.
	 * @param ctx the parse tree
	 */
	void enterLiteral_expression(langParser.Literal_expressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code literal_expression}
	 * labeled alternative in {@link langParser#primary_expression}.
	 * @param ctx the parse tree
	 */
	void exitLiteral_expression(langParser.Literal_expressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code variable_expression}
	 * labeled alternative in {@link langParser#primary_expression}.
	 * @param ctx the parse tree
	 */
	void enterVariable_expression(langParser.Variable_expressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code variable_expression}
	 * labeled alternative in {@link langParser#primary_expression}.
	 * @param ctx the parse tree
	 */
	void exitVariable_expression(langParser.Variable_expressionContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#variable_reference}.
	 * @param ctx the parse tree
	 */
	void enterVariable_reference(langParser.Variable_referenceContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#variable_reference}.
	 * @param ctx the parse tree
	 */
	void exitVariable_reference(langParser.Variable_referenceContext ctx);
	/**
	 * Enter a parse tree produced by the {@code int}
	 * labeled alternative in {@link langParser#literal}.
	 * @param ctx the parse tree
	 */
	void enterInt(langParser.IntContext ctx);
	/**
	 * Exit a parse tree produced by the {@code int}
	 * labeled alternative in {@link langParser#literal}.
	 * @param ctx the parse tree
	 */
	void exitInt(langParser.IntContext ctx);
	/**
	 * Enter a parse tree produced by the {@code float}
	 * labeled alternative in {@link langParser#literal}.
	 * @param ctx the parse tree
	 */
	void enterFloat(langParser.FloatContext ctx);
	/**
	 * Exit a parse tree produced by the {@code float}
	 * labeled alternative in {@link langParser#literal}.
	 * @param ctx the parse tree
	 */
	void exitFloat(langParser.FloatContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#let_binding}.
	 * @param ctx the parse tree
	 */
	void enterLet_binding(langParser.Let_bindingContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#let_binding}.
	 * @param ctx the parse tree
	 */
	void exitLet_binding(langParser.Let_bindingContext ctx);
	/**
	 * Enter a parse tree produced by the {@code variable}
	 * labeled alternative in {@link langParser#pattern}.
	 * @param ctx the parse tree
	 */
	void enterVariable(langParser.VariableContext ctx);
	/**
	 * Exit a parse tree produced by the {@code variable}
	 * labeled alternative in {@link langParser#pattern}.
	 * @param ctx the parse tree
	 */
	void exitVariable(langParser.VariableContext ctx);
	/**
	 * Enter a parse tree produced by the {@code function_signature_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void enterFunction_signature_type(langParser.Function_signature_typeContext ctx);
	/**
	 * Exit a parse tree produced by the {@code function_signature_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void exitFunction_signature_type(langParser.Function_signature_typeContext ctx);
	/**
	 * Enter a parse tree produced by the {@code parenthisised_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void enterParenthisised_type(langParser.Parenthisised_typeContext ctx);
	/**
	 * Exit a parse tree produced by the {@code parenthisised_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void exitParenthisised_type(langParser.Parenthisised_typeContext ctx);
	/**
	 * Enter a parse tree produced by the {@code primitive_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void enterPrimitive_type(langParser.Primitive_typeContext ctx);
	/**
	 * Exit a parse tree produced by the {@code primitive_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void exitPrimitive_type(langParser.Primitive_typeContext ctx);
	/**
	 * Enter a parse tree produced by the {@code tuple_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void enterTuple_type(langParser.Tuple_typeContext ctx);
	/**
	 * Exit a parse tree produced by the {@code tuple_type}
	 * labeled alternative in {@link langParser#type_expression}.
	 * @param ctx the parse tree
	 */
	void exitTuple_type(langParser.Tuple_typeContext ctx);
	/**
	 * Enter a parse tree produced by {@link langParser#simple_type}.
	 * @param ctx the parse tree
	 */
	void enterSimple_type(langParser.Simple_typeContext ctx);
	/**
	 * Exit a parse tree produced by {@link langParser#simple_type}.
	 * @param ctx the parse tree
	 */
	void exitSimple_type(langParser.Simple_typeContext ctx);
}