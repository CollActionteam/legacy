import * as React from "react";
import renderComponentIf from "./renderComponentIf";

interface IMobileNavDrawProps {
  isLoggedIn: boolean;
}

interface IMobileNavDrawState {
  open: boolean;
}

function AccountManageNav(props) {
  if (props.isLoggedIn) {
    return <li><a href="/manage">Account</a></li>;
  }else {
    return null;
  }
 }

 function LoginStateNav(props) {
  if (props.isLoggedIn) {
    return <li><button type="button" onClick={() => document.getElementById("logOutBtn").click()}>Logout</button></li>;
  }else {
    return <li><a href="/account/login">Login</a></li>;
  }
 }

export default class MobileNavDraw extends React.Component<IMobileNavDrawProps, IMobileNavDrawState> {

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
            <AccountManageNav isLoggedIn={this.props.isLoggedIn} />
            <LoginStateNav isLoggedIn={this.props.isLoggedIn} />
        </ul>
      </div>
    );
  }

  render () {
    const self = this;
    return (
      <div className="mobile-header">
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
  <MobileNavDraw
    isLoggedIn={document.getElementById("logOutBtn") ? true : false}
  />,
  document.getElementById("mobile-nav-draw")
);
