﻿using Microsoft.AspNetCore.Mvc;
using YourBrand.Payments.Domain;

namespace YourBrand.Payments.Application;

public static class ResultExtensions
{
    public static ActionResult Handle(this Result result, Func<ActionResult> onSuccess, Func<Error, ActionResult> onError) => result.IsFailure ? onError(result) : onSuccess();

    public static ActionResult Handle<T>(this Result<T> result, Func<T, ActionResult> onSuccess, Func<Error, ActionResult> onError) => result.IsFailure ? onError(result) : onSuccess(result);
}
