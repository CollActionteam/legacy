import * as React from "react";
import renderComponentIf from "./renderComponentIf";

interface IMobileNavDrawState {
  open: boolean;
}

export default class MobileNavDraw extends React.Component<null, IMobileNavDrawState> {

  constructor () {
    super();
    this.state = { open: false };
  }

  open() {
    this.setState({open: true});
  }

  close() {
    this.setState({open: false});
  }

  renderMenu () {
    const self = this;
    return(
      <div id="draw-menu">
        <div id="draw-menu-close-button" onClick={() => self.close()}>
          <i className="fa fa-times" aria-hidden="true"></i>
        </div>
        <ul>
            <li><a href="/">Home</a></li>
            <li><a href="/find">Find Project</a></li>
            <li><a href="/start">Start Project</a></li>
            <li><a href="/about">About</a></li>
            <li><a href="/login">Login</a></li>
        </ul>
      </div>
    );
  }

  render () {
    const self = this;
    return (
      <div>
        <i className="fa fa-bars"
          id="hamburger-icon"
          aria-hidden="true"
          onClick={() => self.open()}>
        </i>
        { this.state.open ? this.renderMenu() : null }
      </div>
    );
  }
}

renderComponentIf(
  <MobileNavDraw />,
  document.getElementById("mobile-nav-draw")
);
