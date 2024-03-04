FROM node:20.8-alpine as build

WORKDIR /app/

ENV REACT_APP_SERVER_HOST_URL "https://localhost:5000"

COPY ["SongSwap React app/ClientApp/aspnetcore-https.js", "./"]
COPY ["SongSwap React app/ClientApp/aspnetcore-react.js", "./"]

COPY ["SongSwap React app/ClientApp/package.json", "SongSwap React app/ClientApp/package-lock.json", "./"]
RUN npm install

COPY ["SongSwap React app/ClientApp/public/", "./public"]
COPY ["SongSwap React app/ClientApp/src/", "./src"]

RUN npm run build


FROM nginx:1.19.0

COPY --from=build /app/build /usr/share/nginx/html
COPY ["SongSwap React app/ClientApp/nginx.conf", "/etc/nginx/nginx.conf"]

EXPOSE 80/tcp

CMD ["/usr/sbin/nginx", "-g", "daemon off;"]