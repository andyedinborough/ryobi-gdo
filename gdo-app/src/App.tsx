import * as React from "react";
import { Layout } from "./Layout";
import "./App.css";
import { Switch, Route } from "react-router";
import { BrowserRouter } from "react-router-dom";
import { Home } from "./Home";
import { LoginSection } from "./Login";

class App extends React.Component {
  public render() {
    return (
      <Layout>
        <BrowserRouter>
          <Switch>
            <Route path="/login" component={LoginSection} />
            <Route component={Home} />
          </Switch>
        </BrowserRouter>
      </Layout>
    );
  }
}

export default App;
