CREATE TABLE IF NOT EXISTS dotnet_aot_api.todos (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id bigint references dotnet_aot_api.users NOT NULL ON DELETE CASCADE,
    content text NOT NULL,
    status varchar(16) NOT NULL,
    created_at timestamp with time zone NOT NULL,
    updated_at timestamp with time zone,
    finished_at timestamp with time zone
);
