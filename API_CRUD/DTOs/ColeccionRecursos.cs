﻿namespace API_CRUD.DTOs
{
    public class ColeccionRecursos<T>: Recurso where T : Recurso

    {
        public List<T> Valores { get; set; }
    }
}
