# Vigil Streamer

MediaMTX streaming server configuration

## Overview

This submodule project handles the broadcasting of the incoming RTSP stream to work with the WebRTC protocol stack. The crucial element is MediaMTX, the media router (proxy + server).

## Quick Start

Use the `.env.example` to make your own `.env` for MediaMTX to consume

```bash
docker-compose up -d
```

## Contents

- `mediamtx/` - MediaMTX configuration files
- `scripts/` - Helper scripts
- `webrtc.html` - WebRTC playback demo
- `docker-compose.yml` - Container orchestration

## Ports

- `8554` - RTSP
- `8889` - WebRTC/HTTP

## Usage

If you would like, you could create your own RTSP stream using ffmpeg to test this out
```bash
ffmpeg -i input.mp4 -f rtsp rtsp://localhost:8554/cam1
```

### Run demo
Open `webrtc.html` in your browser.