import * as React from "react";

interface LayoutProps {}
interface LayoutState {}

export class Layout extends React.Component<LayoutProps, LayoutState> {
  public render() {
    return <div>{this.props.children}</div>;
  }
}
