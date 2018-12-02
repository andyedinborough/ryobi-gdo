import * as React from 'react';
import { withRouter, RouteComponentProps } from 'react-router';
import { store } from './Storage';
import { ryobiClient } from './RyobiClient';
import { Device } from './Device';
import { DeviceStatus } from './DeviceStatus';

interface HomeState {
    devices?: Device[];
    statuses: { [key: string]: DeviceStatus };
}
interface HomeProps extends RouteComponentProps<{}> {}

class HomeBase extends React.Component<HomeProps, HomeState> {
    public state: HomeState = { statuses: {} };

    public componentDidMount() {
        this.load();
    }

    public render() {
        const { devices } = this.state;
        return <div>{devices && devices.map(this.renderDevice)}</div>;
    }

    private renderDevice = (device: Device) => {
        const status = this.state.statuses[device.id];
        return (
            <div className="card">
                <div className="card-body">
                    <div className="card-title">{device.name}</div>
                    {status && (
                        <p>
                            {status.doorIsOpen
                                ? 'Door open. '
                                : 'Door closed. '}
                            {status.lightIsOn ? 'Light on. ' : 'Light off. '}
                        </p>
                    )}
                    <button
                        className="btn btn-link card-link"
                        data-device={device.id}
                        data-part="door"
                        data-value="true"
                        onClick={this.handleActClick}
                    >
                        Open Door
                    </button>
                    <button
                        className="btn btn-link card-link"
                        data-device={device.id}
                        data-part="door"
                        data-value="false"
                        onClick={this.handleActClick}
                    >
                        Close Door
                    </button>
                    <button
                        className="btn btn-link card-link"
                        data-device={device.id}
                        data-part="light"
                        data-value="true"
                        onClick={this.handleActClick}
                    >
                        Light On
                    </button>
                    <button
                        className="btn btn-link card-link"
                        data-device={device.id}
                        data-part="light"
                        data-value="false"
                        onClick={this.handleActClick}
                    >
                        Light Off
                    </button>
                </div>
            </div>
        );
    };

    private handleActClick = (e: React.MouseEvent<HTMLButtonElement>) => {
        const { dataset } = e.currentTarget;
        const device = dataset.device || '';
        const value = /true/i.test(dataset.value || '');
        const part = dataset.part || '';

        const { email, apiKey } = store.get();
        if (!email || !apiKey) {
            alert('cannot operate');
            return;
        }

        const status = this.state.statuses[device];
        ryobiClient.operate(
            email,
            apiKey,
            device,
            status.moduleId,
            status.portId,
            { [part]: value }
        );
    };

    private async load() {
        const { cookie, email, password } = store.get();

        if (!email || !password) {
            this.props.history.push('/login');
            return;
        }

        if (!cookie) {
            const result = await ryobiClient.login(email, password);
            if (!result) {
                return;
            }
            await this.load();
            return;
        }

        const devices = await ryobiClient.devices(cookie);
        this.setState({ devices });

        const deviceStatusPromises = devices.map(x =>
            ryobiClient.getStatus(cookie, x.id)
        );
        const deviceStatuses = await Promise.all(deviceStatusPromises);
        const { statuses } = this.state;
        for (let i = 0; i < devices.length; i++) {
            const status = deviceStatuses[i];
            const device = devices[i];
            statuses[device.id] = status;
        }
        this.setState({ statuses });
    }
}

export const Home = withRouter(HomeBase);
