﻿namespace Derby.Backend.Errors;

public abstract record DerbyError(string Message);

public record NotFoundError(string Message) : DerbyError(Message);
public record BadRequestError(string Message) : DerbyError(Message);

public record UnauthorizedError(string Message) : DerbyError(Message);
public record InternalServerError(string Message) : DerbyError(Message);

public record EquipoYaInscritoError(string Message) : DerbyError(Message);