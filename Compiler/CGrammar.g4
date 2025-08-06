grammar CGrammar;

ID: [a-zA-Z_][a-zA-Z0-9_]*;
INT: [0-9]+;

program
    : function EOF
    ;
    
function
    : 'int' ID '(' ')' '{' statement* '}'
    ;
    
statement
    : 'return' expression ';' #returnStatement
    ;

expression
    : '(' expression ')' #parenthesizedExpression
    
    | '!' expression #notExpression
    | '~' expression #invertExpression
    
    | left=expression '*' right=expression #multiplyExpression
    | left=expression '/' right=expression #divideExpression
    | left=expression '%' right=expression #modulusExpression
    
    | left=expression '+' right=expression #addExpression
    | left=expression '-' right=expression #subtractExpression
    
    | left=expression '<' right=expression #lessThanExpression
    | left=expression '>' right=expression #greaterThanExpression
    | left=expression '<=' right=expression #lessThanOrEqualExpression
    | left=expression '>=' right=expression #greaterThanOrEqualExpression
    | left=expression '==' right=expression #equalExpression
    | left=expression '!=' right=expression #notEqualExpression
    
    | left=expression '&' right=expression #bitwiseAndExpression
    | left=expression '|' right=expression #bitwiseOrExpression
    | left=expression '^' right=expression #bitwiseXorExpression
    
    | left=expression '&&' right=expression #logicalAndExpression
    | left=expression '||' right=expression #logicalOrExpression
    
    | constant #constantExpression
    ;
    
constant
    : INT # intConstant
    ;