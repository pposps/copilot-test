-- Create health table
CREATE TABLE IF NOT EXISTS health (
    id SERIAL PRIMARY KEY,
    status VARCHAR(255) NOT NULL UNIQUE
);

-- Insert initial data (will be skipped if 'healthy' status already exists)
INSERT INTO health (status) VALUES ('healthy') ON CONFLICT (status) DO NOTHING;
