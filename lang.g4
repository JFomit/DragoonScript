grammar lang;

file: declaration* EOF;

declaration:
  type_declaration
  | function_declaration
  | let_binding;

// **************** * DECLARATIONS * ****************

type_declaration: TYPE name = IDENTIFIER;
function_declaration:
  FN name = IDENTIFIER params = parameter_list (
    COLON signature = type_expression
  )? IS body = primary_expression;
parameter_list: head = parameter tail = parameter_list?;
parameter: IDENTIFIER;

// **************** * EXPRESSIONS * ****************

primary_expression:
  LEFT_PARENTHESIS primary_expression RIGHT_PARENTHESIS # parenthisised_expression
  | literal # literal_expression
  | variable_reference # variable_expression;
variable_reference: name = IDENTIFIER;
literal: INTEGER # int | FLOAT # float;
let_binding:
  LET binding_pattern = pattern IS value = primary_expression;
pattern: variable = IDENTIFIER # variable;

// **************** * TYPES * ****************

type_expression:
  LEFT_PARENTHESIS type_expression RIGHT_PARENTHESIS # parenthisised_type
  | type_expression SIGNATURE_ARROW type_expression # function_signature_type
  | type_expression MULTIPLY type_expression # tuple_type
  | simple_type # primitive_type;
simple_type: IDENTIFIER;

NEW_LINE: '\n';

WS: [ \t]; // whitespace

LEFT_PARENTHESIS: '(';
RIGHT_PARENTHESIS: ')';

MULTIPLY: '*';
SIGNATURE_ARROW: '->';
LAMBDA_ARROW: '=>';
COLON: ':';
FN: 'fn';
LET: 'let';
TYPE: 'type';
IS: '=';

ABSURD: '!';

IDENTIFIER: [a-zA-Z_][a-zA-Z_0-9]*;
UNIT: '()';
INTEGER: [0-9]+;
FLOAT: [0-9]* '.' [0-9]+;

fragment GRAVE: '`';
OPERATOR: [!#$%&*+./<=>?@\\^|-~]+ | GRAVE IDENTIFIER GRAVE;