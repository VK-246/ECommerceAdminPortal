# Stage 1: Build the Angular app
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --configuration=production

# Stage 2: Serve the app with Nginx
FROM nginx:alpine
# Copy the custom Nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Copy the built output (make sure the dist folder name matches your angular.json output path)
# Angular 17+ creates dist/project-name/browser
COPY --from=build /app/dist/frontend/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
