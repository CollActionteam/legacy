import renderComponentIf from "../global/renderComponentIf";
import * as React from "react";
import * as ReactDOM from 'react-dom';
import {IdealBankElement} from 'react-stripe-elements';

interface IIdealBoxProps {
    stripe: any;
    amount: number;
    userEmail: string;
    userName: string;
}

interface IIdealBoxState {
    showError: boolean;
    error: string;
}

export default class IdealBox extends React.Component<IIdealBoxProps, IIdealBoxState> {
    constructor(props) {
        super(props);
        this.state = {
            showError: false,
            error: ""
        };
    }

    async submitPayment(event: React.FormEvent<HTMLFormElement>) {
        event.preventDefault();
        this.setState({ showError: false });

        let sourceData = {
            type: "ideal",
            amount: this.props.amount * 100,
            currency: "eur",
            owner: {
                name: this.props.userName,
                email: this.props.userEmail
            },
            redirect: {
                return_url: window.location.origin + "/donation/Return"
            }
        };

        let response = await this.props.stripe.createSource(sourceData);
        if (response.error) {
            console.log("Unable to start iDeal: " + response.error);
            this.setState({ showError: true, error: "Unable to start iDeal" })
            return;
        }

        let initializeUrl = `/donation/InitializeIdealCheckout?sourceId=${response.source.id}&name=${encodeURIComponent(this.props.userName)}&email=${encodeURIComponent(this.props.userEmail)}`;
        let initializeResponse = await fetch(initializeUrl, { method: 'POST' });
        if (initializeResponse.status == 200) {
            window.location.href = response.source.redirect.url;
        } else {
            console.log("Unable to start iDeal: " + await initializeResponse.text());
            this.setState({ showError: true, error: "Unable to start iDeal" })
        }
    }

    render() {
        return (
            <form onSubmit={(ev) => this.submitPayment(ev)}>
                <div className="error" hidden={!this.state.showError}>
                    <p>{this.state.error}</p>
                </div>
                <IdealBankElement />
                <input type="submit" className="btn btn-default" value="submit" />
            </form>);
    }
} 