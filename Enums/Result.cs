using System.Diagnostics;

namespace Rusting.Enums;

public abstract record Result<T, E>
{
	public record Ok(T Value) : Result<T, E>;

	public record Err(E Value) : Result<T, E>;

	public T Unwrap() =>
		this is Ok(T value) ? value : throw new Exception("Calling `Unwrap` on a `Ok`");

	public E UnwrapErr() =>
		this is Err(E value) ? value : throw new Exception("Calling `UnwrapErr` on a `Err`");

	public bool IsOk() => this is Ok;

	public bool IsErr() => this is Err;

	public Result<U, E> And<U>(Result<U, E> res)
	{
		return this switch
		{
			Ok => res,
			Err(E value) => new Result<U, E>.Err(value),
			_ => throw new UnreachableException(),
		};
	}

	public Result<U, E> AndThen<U>(SingleParamFunc<T, Result<U, E>> func)
	{
		return this switch
		{
			Ok(T value) => func(value),
			Err(E value) => new Result<U, E>.Err(value),
			_ => throw new UnreachableException(),
		};
	}

	public T Expect(string message) => this is Ok(T value) ? value : throw new Exception(message);

	public E ExpectErr(string message) =>
		this is Err(E value) ? value : throw new Exception(message);

	public bool IsErrAnd(SingleParamFunc<E, bool> func) => this is Err(E value) && func(value);

	public bool IsOkAnd(SingleParamFunc<T, bool> func) => this is Ok(T value) && func(value);

	public Result<U, E> Map<U>(SingleParamFunc<T, U> func)
	{
		return this switch
		{
			Ok(T value) => new Result<U, E>.Ok(func(value)),
			Err(E value) => new Result<U, E>.Err(value),
			_ => throw new UnreachableException(),
		};
	}

	public Result<T, F> MapErr<F>(SingleParamFunc<E, F> func)
	{
		return this switch
		{
			Ok(T value) => new Result<T, F>.Ok(value),
			Err(E value) => new Result<T, F>.Err(func(value)),
			_ => throw new UnreachableException(),
		};
	}

	public U MapOr<U>(U def, SingleParamFunc<T, U> func)
	{
		return this switch
		{
			Ok(T value) => func(value),
			Err => def,
			_ => throw new UnreachableException(),
		};
	}

	public U MapOrElse<U>(SingleParamFunc<E, U> err, SingleParamFunc<T, U> ok)
	{
		return this switch
		{
			Ok(T value) => ok(value),
			Err(E value) => err(value),
			_ => throw new UnreachableException(),
		};
	}

	public Result<T, F> Or<F>(Result<T, F> res)
	{
		return this switch
		{
			Ok(T value) => new Result<T, F>.Ok(value),
			Err => res,
			_ => throw new UnreachableException(),
		};
	}

	public Result<T, F> OrElse<F>(SingleParamFunc<E, Result<T, F>> func)
	{
		return this switch
		{
			Ok(T value) => new Result<T, F>.Ok(value),
			Err(E value) => func(value),
			_ => throw new UnreachableException(),
		};
	}

	public T UnwrapOr(T def)
	{
		return this switch
		{
			Ok(T value) => value,
			Err => def,
			_ => throw new UnreachableException(),
		};
	}

	public T UnwrapOrElse(SingleParamFunc<E, T> func)
	{
		return this switch
		{
			Ok(T value) => value,
			Err(E value) => func(value),
			_ => throw new UnreachableException(),
		};
	}

	public Option<T> OptionOk()
	{
		return this switch
		{
			Ok(T value) => new Option<T>.Some(value),
			_ => new Option<T>.None(),
		};
	}

	public Option<E> OptionErr()
	{
		return this switch
		{
			Err(E err) => new Option<E>.Some(err),
			_ => new Option<E>.None(),
		};
	}
}

public static class ResultExt
{
	public static T Throw<T, E>(this Result<T, E> result)
		where E : Exception
	{
		return result switch
		{
			Result<T, E>.Ok(T value) => value,
			Result<T, E>.Err(E value) => throw value,
			_ => throw new UnreachableException(),
		};
	}

	public static Result<T, E> CatchUnwind<T, E>(Func<T> func)
		where E : Exception
	{
		try
		{
			return new Result<T, E>.Ok(func());
		}
		catch (E error)
		{
			return new Result<T, E>.Err(error);
		}
	}

	public static async Task<Result<T, E>> CatchUnwindAsync<T, E>(Func<Task<T>> func)
		where E : Exception
	{
		try
		{
			return new Result<T, E>.Ok(await func());
		}
		catch (E error)
		{
			return new Result<T, E>.Err(error);
		}
	}
}
