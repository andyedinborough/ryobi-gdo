interface Data {
    email?: string;
    password?: string;
    cookie?: string;
    apiKey?: string;
}

class Storage {
    get() {
        try {
            return JSON.parse(localStorage.getItem('data') || '{}') as Data;
        } catch {}
        return {} as Data;
    }
    set(data: Data) {
        const newData = { ...this.get(), ...data };
        localStorage.setItem('data', JSON.stringify(newData));
    }
}

export const store = new Storage();
