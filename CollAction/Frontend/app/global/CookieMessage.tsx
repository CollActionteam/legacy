import * as React from "react";
import renderComponentIf from "./renderComponentIf";

interface ICookieMessageProps {
}

interface ICookieMessageState {
    accepted: Date;
}

export default class CookieMessage extends React.Component<ICookieMessageProps, ICookieMessageState> {
    constructor(props) {
        super(props);
        this.state = { accepted: null };
    }

    componentDidMount() {
        this.setState({
            accepted: this.getCookiesAcceptedDate()
        });
    }

    getCookiesAcceptedDate(): Date {
        let cookieAccepted = document.cookie
            .split(";")
            .filter(function(item) {
                return item.indexOf("cookiesAccepted=") >= 0;
            })
            .map(function(item) {
                return item.split("=")[1];
            });

        if (cookieAccepted.length === 0) {
            return null;
        }

        let acceptedOn = new Date(cookieAccepted[0]);
        if (isNaN(acceptedOn.getTime())) {
            return null;
        }

        return acceptedOn;
    }

    accept() {
        document.cookie = "cookiesAccepted=" + new Date() + "; expires=Sun, 15 Jun 2177 23:59:59 GMT";
        this.setState({
            accepted: new Date()
        });
    };

    renderMessage() {
        return (
            <div>
                <div id="cookie-message-banner">
                    <div className="container">
                        <div className="row">
                            <div className="col-xs-12">
                                <p>Thank you for visiting our website! Please note that our website uses cookies to analyze and improve the performance of our website and to make social media integration possible. For more information on the use of cookies and privacy related matters, please see our <a href="home/privacy">Privacy and Cookies Policy</a>. By clicking "I accept" or by using this site, you consent to the use of cookies.</p>
                                <p><a className="btn" href="#" onClick={() => this.accept()}>Yes, I accept cookies</a></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    render() {
        if (this.state.accepted === null) {
            return this.renderMessage();
        }
        else {
            return (<div></div>);
        }
    }
}

renderComponentIf(
    <CookieMessage />,
    document.getElementById("cookie-message")
);
