# docker-compose build robotic-arm-service && docker-compose up -d robotic-arm-service && docker-compose logs -f robotic-arm-service
# docker-compose build event-hkandler && docker-compose up -d event-handler && docker-compose logs -f event-handler
docker-compose build monitoring && docker-compose up -d monitoring && docker-compose logs -f monitoring
# docker-compose build storage-middleware-api && docker-compose up -d storage-middleware-api && docker-compose logs -f storage-middlekware-api
# docker-compose build cassandra && docker-compose up -d cassandra && docker-compose logs -f cassandra