-- Create health table
CREATE TABLE IF NOT EXISTS health (
    id SERIAL PRIMARY KEY,
    status VARCHAR(255) NOT NULL
);

-- Insert initial data
INSERT INTO health (status) VALUES ('healthy') ON CONFLICT DO NOTHING;
