FROM node:22-alpine3.20
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build --prod
CMD [ "npx", "serve", "-s", "build" ]