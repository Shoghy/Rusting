namespace Rusting.Enums;

public abstract record Option<T>
{
	public record Some(T Value) : Option<T>;

	public record None : Option<T>;

	public bool IsSome() => this is Some;

	public bool IsSomeAnd(SingleParamFunc<T, bool> func) => this is Some(T value) && func(value);

	public bool IsNone() => this is None;

	public bool IsNoneOr(SingleParamFunc<T, bool> func) => this is not Some(T value) || func(value);

	public Option<T> Inspect(SingleParamVoidFunc<T> func)
	{
		if (this is Some(T value))
		{
			func(value);
		}
		return this;
	}

	public Option<T> Or(Option<T> optb) => this is Some ? this : optb;

	public Option<T> OrElse(Func<Option<T>> func) => this is Some ? this : func();

	public Option<T> Xor(Option<T> optb)
	{
		return (this, optb) switch
		{
			(Some a, None) => a,
			(None, Some b) => b,
			_ => new None(),
		};
	}

	public Option<U> And<U>(Option<U> optb)
	{
		return (this, optb) switch
		{
			(Some, Option<U>.Some) => optb,
			_ => new Option<U>.None(),
		};
	}

	public Option<U> AndThen<U>(SingleParamFunc<T, Option<U>> func) =>
		this is Some(T value) ? func(value) : new Option<U>.None();

	public T Expect(string msg) => this is Some(T value) ? value : throw new Exception(msg);

	public T Unwrap() =>
		this is Some(T value) ? value : throw new Exception("Calling `Unwrap` on a `None`");

	public T UnwarpOr(T def) => this is Some(T value) ? value : def;

	public T UnwrapOrElse(Func<T> func) => this is Some(T value) ? value : func();

	public Option<(T, U)> Zip<U>(Option<U> other)
	{
		return (this, other) switch
		{
			(Some(T a), Option<U>.Some(U b)) => new Option<(T, U)>.Some((a, b)),
			_ => new Option<(T, U)>.None(),
		};
	}

	public Option<U> Map<U>(SingleParamFunc<T, U> func) =>
		this is Some(T value) ? new Option<U>.Some(func(value)) : new Option<U>.None();

	public U MapOr<U>(U def, SingleParamFunc<T, U> func) =>
		this is Some(T value) ? func(value) : def;

	public U MapOrElse<U>(Func<U> def, SingleParamFunc<T, U> func) =>
		this is Some(T value) ? func(value) : def();

	public Result<T, E> OkOr<E>(E err) =>
		this is Some(T value) ? new Result<T, E>.Ok(value) : new Result<T, E>.Err(err);

	public Result<T, E> OkOrElse<E>(Func<E> func) =>
		this is Some(T value) ? new Result<T, E>.Ok(value) : new Result<T, E>.Err(func());
}
