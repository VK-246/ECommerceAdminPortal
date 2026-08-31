# Stage 1: Build the Angular app
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
# -- separator passes the flag to Angular CLI (ng build), not to npm itself
RUN npm run build -- --configuration=production

# Stage 2: Serve the app with Nginx
FROM nginx:alpine
# Copy the custom Nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Copy the built output (matches angular.json outputPath + /browser)
COPY --from=build /app/dist/ecommerce-admin/browser /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
