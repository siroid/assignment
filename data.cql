CREATE KEYSPACE test_keyspace WITH replication = {'class': 'SimpleStrategy', 'replication_factor' : 1};

CREATE TABLE test_keyspace.key_value (
    key text PRIMARY KEY,
    value text
);