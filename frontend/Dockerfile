# Development stage
FROM node:22-alpine
WORKDIR /app

# Copy package files for the root and all workspaces
COPY package*.json ./
COPY lerna.json ./

# Copy package.json files from all workspaces
COPY campaigns/package*.json ./campaigns/
COPY host/package*.json ./host/
COPY registration/package*.json ./registration/
COPY scheduling/package*.json ./scheduling/

# Install dependencies (including lerna and all workspace dependencies)
RUN npm ci

# Copy the rest of the source code
COPY . .

# Build all micro-frontends
RUN npm run build

# Nginx stage
FROM nginx:alpine
COPY --from=0 /app/campaigns/dist /usr/share/nginx/html/dist/campaigns
COPY --from=0 /app/host/dist /usr/share/nginx/html/dist/host
COPY --from=0 /app/registration/dist /usr/share/nginx/html/dist/registration
COPY --from=0 /app/scheduling/dist /usr/share/nginx/html/dist/scheduling

# Copy nginx config
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
