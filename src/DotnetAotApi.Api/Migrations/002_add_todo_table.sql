IF NOT EXISTS (SELECT FROM pg_catalog.pg_tables WHERE schemaname = 'dotnet_aot_api' AND tablename = 'todos') THEN
    CREATE TABLE dotnet_aot_api.todos (
        id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        user_id bigint references dotnet_aot_api.users NOT NULL ON DELETE CASCADE,
        content text NOT NULL,
        status varchar(16) NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        finished_at timestamp with time zone
    );

    RAISE NOTICE 'Applied migration 002_add_todo_table';
END IF
