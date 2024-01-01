FROM node:16-alpine

WORKDIR /TaskManager
COPY package*.json ./
COPY . .

ARG API_URL=http://localhost:5000
ENV API_URL=${API_URL}

RUN npm install

EXPOSE 4200

CMD ["npm", "start"]  # Start the application