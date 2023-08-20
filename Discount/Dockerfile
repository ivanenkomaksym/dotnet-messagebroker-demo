# syntax=docker/dockerfile:1

FROM golang:alpine AS builder

# Set destination for COPY
WORKDIR /app

# Download Go modules
COPY Discount/go.mod Discount/go.sum ./
RUN go mod download

# Copy the source code. Note the slash at the end, as explained in
# https://docs.docker.com/engine/reference/builder/#copy
COPY Discount/cmd ./cmd
COPY Discount/internal ./internal

# Build
RUN CGO_ENABLED=0 GOOS=linux go build -o /docker-gs-ping ./cmd/main.go

# Optional:
# To bind to a TCP port, runtime parameters must be supplied to the docker command.
# But we can document in the Dockerfile what ports
# the application is going to listen on by default.
# https://docs.docker.com/engine/reference/builder/#expose
EXPOSE 80

# Run
CMD ["/docker-gs-ping"]