﻿using Microsoft.Extensions.Logging;

namespace KR.Document.HB.Adapter;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId =3, EventName ="BlobUploadStart",Level = LogLevel.Information, Message ="Blob upload started for blob {blob} in container {container}")]
    public static partial void LogBlobUploadStart(this ILogger logger, string blob, string container);

    [LoggerMessage(EventId =3, EventName ="BlobUploadComplete",Level = LogLevel.Information, Message ="Blob upload completed for blob {blob} in container {container} generated blob version {blobSequence}")]
    public static partial void LogBlobUploadComplete(this ILogger logger, string blob, string container, string blobSequence);

    [LoggerMessage(EventId =3, EventName ="LogBlogDownloadStart",Level = LogLevel.Information, Message ="Blob download started for blob {blob} in container {container}")]
    public static partial void LogBlogDownloadStart(this ILogger logger, string blob, string container);

    [LoggerMessage(EventId =3, EventName ="LogBlogDownloadComplete",Level = LogLevel.Information, Message ="Blob download completed for blob {blob} in container {container}")]
    public static partial void LogBlogDownloadComplete(this ILogger logger, string blob, string container);



}
