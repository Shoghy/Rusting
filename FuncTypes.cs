namespace Rusting;

public delegate Return SingleParamFunc<ParamType, Return>(ParamType value);

public delegate void SingleParamVoidFunc<ParamType>(ParamType value);

public delegate void FuncVoid();
