import * as React from "react";
import renderComponentIf from "./renderComponentIf";
import Drawer from "material-ui/Drawer";
import MuiThemeProvider from "material-ui/styles/MuiThemeProvider";

interface IMobileNavDrawState {
  open: boolean;
}

export default class MobileNavDraw extends React.Component<null, IMobileNavDrawState> {

  constructor () {
    super();
    this.state = { open: false };
  }

  onClick() {
    this.setState({
      open: !this.state.open
    });
  }

  render () {
    return (
      <div>
        <i className="fa fa-bars"
          id="hamburger-icon"
          aria-hidden="true"
          onClick={() => this.onClick()}>
        </i>

        <Drawer
          docked={false}
          width={200}
          open={this.state.open}
          onRequestChange={(open) => this.setState({open})}
          >
          <ul id="draw-menu">
              <li><a href="/find">Find Project</a></li>
              <li><a href="/start">Start Project</a></li>
              <li><a href="/about">About</a></li>
          </ul>
        </Drawer>
      </div>
    );
  }
}

renderComponentIf(
  <MuiThemeProvider><MobileNavDraw /></MuiThemeProvider>,
  document.getElementById("mobile-nav-draw")
);
