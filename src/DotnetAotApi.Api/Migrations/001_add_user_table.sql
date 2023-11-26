IF NOT EXISTS (SELECT FROM pg_catalog.pg_tables WHERE schemaname = 'dotnet_aot_api' AND tablename = 'users') THEN
    CREATE TABLE dotnet_aot_api.users (
        id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
        name text NOT NULL,
        email text UNIQUE NOT NULL,
        status varchar(16) NOT NULL,
        password_hash text NOT NULL,
        created_at timestamp with time zone NOT NULL,
        updated_at timestamp with time zone,
        deleted_at timestamp with time zone
    );

    RAISE NOTICE 'Applied migration 001_add_user_table';
END IF
