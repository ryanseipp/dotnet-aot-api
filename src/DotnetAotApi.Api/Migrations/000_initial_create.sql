IF NOT EXISTS (SELECT 1 FROM pg_namespace WHERE nspname = 'dotnet_aot_api') THEN
    CREATE SCHEMA IF NOT EXISTS dotnet_aot_api;

    RAISE NOTICE 'Applied migration 000_initial_create';
END IF
