grammar dragoon;

file: declaration* EOF;

declaration
  : function_declaration
  | let_binding
  // | type_declaration
  ;

function_declaration
  : FN LPAREN name=OPERATOR RPAREN params=parameter_list (COLON type=type_expression) IS body=block_expression
  | FN name=IDENTIFIER params=parameter_list (COLON type=type_expression) IS body=block_expression
  ;
parameter_list
  : parameter*
  ;
parameter
  : IDENTIFIER
  ;
block_expression
  : let_binding* expression
  ;
let_binding
  : LET pattern=binding_pattern IS value=expression IN
  // TODO: implement proper offside support
  | LET pattern=binding_pattern IS value=expression NEW_LINE // And rules:
                                                             // - if line ends with operator, line is continued
                                                             // - if line ends with identifier, line is continued if next token is operator
                                                             // - otherwise, this is the end of let pattern
  ;
expression
  : lhs=primary_expression op=OPERATOR rhs=primary_expression
  | function=expression expression+
  ;
primary_expression
  : match_expression
  | if_expression
  | variable_reference
  | literal
  | LPAREN inner=expression RPAREN
  | prefix_operator
  ;
literal
  : INTEGER
  | FLOAT
  | STRING
  ;
variable_reference
  : IDENTIFIER
  ;
prefix_operator
  : op=OPERATOR rhs=expression
  ;
if_expression
  : IF condition=expression THEN then=block_expression ELSE else_=block_expression
  ;
match_expression
  : MATCH value=expression WITH match_pattern_list
  ;
match_pattern_list
  : (PIPE binding_pattern ARROW block_expression)+
  ;
binding_pattern
  : IDENTIFIER
  ;

type_expression
  : primary_type_expression
  | primary_type_expression ARROW primary_type_expression
  ;
primary_type_expression
  : type_constructor
  | LPAREN inner=type_expression RPAREN
  ;
type_constructor
  : IDENTIFIER primary_type_expression*
  ;

NEW_LINE: '\n';

WS: [ \t]+; // whitespace

LPAREN: '(';
RPAREN: ')';

ARROW: '->';
COLON: ':';
FN: 'fn';
LET: 'let';
TYPE: 'type';
IS: '=';
PIPE: '|';
IF: 'if';
THEN: 'then';
ELSE: 'else';
IN: 'in';
MATCH: 'match';
WITH: 'with';

IDENTIFIER: [a-zA-Z_][a-zA-Z_0-9]*;
UNIT: '()';
INTEGER: [0-9]+;
FLOAT: [0-9]* '.' [0-9]+;

STRING: '"'.*?'"';
OPERATOR: [!#$%&*+./<=>?@\\^|-~]+;