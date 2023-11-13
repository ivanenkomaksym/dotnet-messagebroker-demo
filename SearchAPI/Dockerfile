FROM node:18-alpine

ENV ENVIRONMENT production

# Copy the source code and configuration file into the image.
COPY . /app
COPY SearchAPI/config/$ENVIRONMENT/config.json /app/config.json

# Install the dependencies.
WORKDIR /app
RUN npm install

# Expose the port that the SearchAPI microservice will listen on.
EXPOSE 80

# Start the SearchAPI microservice.
CMD ["node", "./src/index.ts"]