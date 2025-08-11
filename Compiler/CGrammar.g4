grammar CGrammar;

options {
    contextSuperClass = Q1.Compiler.Context;
}

HEX: '0x' [0-9a-fA-F];

ID: [a-zA-Z_][a-zA-Z0-9_]*;
INT: [0-9]+;
WS: [ \t\r\n]+ -> skip;
COMMENT: '//' .*? ([\n] | EOF) -> channel(HIDDEN);


program
    : function+ EOF
    ;
    
function
    : rtype=type name=ID '(' ( types+=type params+=ID (',' types+=type params+=ID )* )? ')' ';' #functionPrototype
    | rtype=type name=ID '(' ( types+=type params+=ID (',' types+=type params+=ID )* )? ')' block #functionDefinition
    ;
   
block
    : '{' block_item* '}'
    ;
    
block_item
    : statement
    | declaration
    ;
    
statement
    : 'return' expression ';' #returnStatement
    | 'if' '(' expression ')' then=statement ('else' else=statement)? #ifStatement
    | 'for' '(' (initializer=expression)? ';' cond=expression? ';' post=expression? ')' body=statement #forExprStatement
    | 'for' '(' initializer=declaration cond=expression? ';' post=expression? ')' body=statement #forDeclStatement
    | 'while' '(' cond=expression ')' body=statement #whileStatement
    | 'do' body=statement 'while' '(' cond=expression ')' ';' #doWhileStatement
    | 'break' ';' #breakStatement
    | 'continue' ';' #continueStatement
    | expression ';' #expressionStatement
    | block #blockStatement
    | ';' #nullStatement
    ;
    
    
declaration
    : type name=ID ('=' value=expression)? ';' #variableDeclaration
    ;

expression
    : '(' expression ')' #parenthesizedExpression
//    | left=expression ',' right=expression #commaExpression
    | constant #constantExpression
    | target '=' expression #assignmentExpression
    | '*' expression #dereferenceExpression
    | '&' target #addressOfExpression
    | ID #variableExpression
    
    | ID '(' ( params+=expression (',' params+=expression )* )? ')' #callExpression
    | obj=expression '[' indexer=expression ']' #indexExpression
    
    | '!' expression #notExpression
    | '~' expression #invertExpression
    
    | '++' target #incrementPrefixExpression
    | '--' target #decrementPrefixExpression
    | target '++' #incrementPostfixExpression
    | target '--' #decrementPostfixExpression
    
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
    
    | left=expression '<<' right=expression #bitwiseLeftShiftExpression
    | left=expression '>>' right=expression #bitwiseRightShiftExpression
    
    | left=expression '&&' right=expression #logicalAndExpression
    | left=expression '||' right=expression #logicalOrExpression
    
    | target '*=' expression #compoundMultiplyExpression
    | target '/=' expression #compoundDivideExpression
    | target '%=' expression #compoundModulusExpression
    
    | target '+=' expression #compoundAddExpression
    | target '-=' expression #compoundSubtractExpression
    
    | target '&=' expression #compoundBitwiseAndExpression
    | target '|=' expression #compoundBitwiseOrExpression
    | target '^=' expression #compoundBitwiseXorExpression
    
    | target '<<=' expression #compoundBitwiseLeftShiftExpression
    | target '>>=' expression #compoundBitwiseRightShiftExpression
    
    | cond=expression '?' then=expression ':' else=expression #ternaryExpression
    ;
    
constant
    : INT # intConstant
    | HEX # hexConstant
    ;
    
type
    : 'int' #intType
    | 'char' #charType
    | type '*' #pointerType
    ;
    
target
    : ID # variableTarget
    ;