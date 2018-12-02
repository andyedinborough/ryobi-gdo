import { store } from './Storage';
import { parseJson } from './JsonHelper';
import { LoginResult } from './LoginResult';
import { Device } from './Device';
import { DeviceStatus } from './DeviceStatus';

const toForm = (body: {}) =>
    Object.keys(body)
        .map(x => encodeURIComponent(x) + '=' + encodeURIComponent(body[x]))
        .join('&');

const formPost = (path: string, body: {}) =>
    fetch(path, {
        method: 'post',
        body: toForm(body),
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    });

const formPostJson = <T>(path: string, body: {}) =>
    formPost(path, body).then(x => parseJson<T>(x));

export class RyobiClient {
    public async login(email: string, password: string) {
        const auth = await formPostJson<LoginResult>('/api/Ryobi/Login', {
            email,
            password
        });
        store.set({ email, password, ...auth });
        return !!auth.apiKey;
    }

    public async devices(cookie: string) {
        const devices = await formPostJson<Device[]>('/api/Ryobi/GetDevices', {
            cookie
        });
        return devices;
    }

    public async getStatus(cookie: string, deviceId: string) {
        const status = await formPostJson<DeviceStatus>(
            '/api/Ryobi/DeviceStatus',
            { id: deviceId, cookie }
        );
        return status;
    }

    public operate(
        email: string,
        apiKey: string,
        deviceId: string,
        moduleId: number,
        portId: number,
        cmd: {}
    ) {
        return fetch('/api/Ryobi/Operate', {
            method: 'post',
            body: `${toForm({
                deviceId,
                email,
                apiKey,
                moduleId,
                portId
            })}&${toForm(cmd)}`,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        });
    }

    public act(
        email: string,
        password: string,
        deviceName: string,
        part: string,
        value: boolean
    ) {
        return fetch('/api/Ryobi/Act', {
            method: 'post',
            body: toForm({ email, password, deviceName, part, value }),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        });
    }
}

export const ryobiClient = new RyobiClient();
