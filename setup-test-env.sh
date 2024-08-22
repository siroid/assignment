# Step 1: Deploy Kafka and Zookeeper
docker-compose up -d

# Step 2: Wait for Kafka to be ready
echo "Waiting for Kafka to be ready..."
while ! docker exec $(docker ps -qf "ancestor=confluentinc/cp-kafka:latest") kafka-topics --bootstrap-server localhost:9092 --list &>/dev/null; do
  sleep 1
done

# Step 3: Create Kafka Topic
docker exec $(docker ps -qf "ancestor=confluentinc/cp-kafka:latest") kafka-topics --create --topic robotic-arm-events --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1

# Step 4: Verify Topic Creation
docker exec $(docker ps -qf "ancestor=confluentinc/cp-kafka:latest") kafka-topics --list --bootstrap-server localhost:9092