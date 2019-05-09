import * as React from "react";
import { IdealBankElement, IbanElement } from "react-stripe-elements";

interface IDebitDetailsBoxProps {
    stripe: any;
    amount: number;
    userEmail: string;
    userName: string;
    isRecurring: boolean;
}

interface IDebitDetailsBoxState {
    showError: boolean;
    error: string;
}

export default class DebitDetailsBox extends React.Component<IDebitDetailsBoxProps, IDebitDetailsBoxState> {
    constructor(props) {
        super(props);
        this.state = {
            showError: false,
            error: ""
        };
    }

    submitPayment(event: React.FormEvent<HTMLFormElement>): Promise<void> {
        event.preventDefault();
        this.setState({ showError: false });

        if (this.props.isRecurring) {
            return this.submitPaymentSepa();
        }
        else {
            return this.submitPaymentIdeal();
        }
    }

    private async submitPaymentSepa(): Promise<void> {
        let sourceData = {
            type: "sepa_debit",
            currency: "eur",
            owner: {
                name: this.props.userName,
                email: this.props.userEmail
            },
            mandate: {
                notification_method: "email" // Automatically send a mandate notification email to your customer once the source is charged
            }
        };

        let response = await this.props.stripe.createSource(sourceData);
        if (response.error) {
            console.log("Unable to start SEPA Direct: " + response.error);
            this.setState({ showError: true, error: "We're unable to setup your SEPA Direct donation, something is wrong, we're sorry" });
            return;
        }

        let initializeUrl = `/Donation/InitializeSepaDirect?sourceId=${response.source.id}&name=${encodeURIComponent(this.props.userName)}&email=${encodeURIComponent(this.props.userEmail)}&amount=${this.props.amount}`;
        let initializeResponse = await fetch(initializeUrl, { method: "POST" });
        if (initializeResponse.status == 200) {
            window.location.href = "/Donation/ThankYou";
        } else {
            console.log("Unable to start SEPA Direct: " + await initializeResponse.text());
            this.setState({ showError: true, error: "We're unable to setup your SEPA Direct donation, something is wrong, we're sorry" });
        }
    }

    private async submitPaymentIdeal(): Promise<void> {
        let sourceData = {
            type: "ideal",
            amount: this.props.amount * 100,
            currency: "eur",
            statement_descriptor: "Donation CollAction",
            owner: {
                name: this.props.userName,
                email: this.props.userEmail
            },
            redirect: {
                return_url: window.location.origin + "/Donation/Return"
            }
        };

        let response = await this.props.stripe.createSource(sourceData);
        if (response.error) {
            console.log("Unable to start iDeal: " + response.error);
            this.setState({ showError: true, error: "We're unable to start your iDeal donation, something is wrong, we're sorry" });
            return;
        }

        let initializeUrl = `/Donation/InitializeIdealCheckout?sourceId=${response.source.id}&name=${encodeURIComponent(this.props.userName)}&email=${encodeURIComponent(this.props.userEmail)}`;
        let initializeResponse = await fetch(initializeUrl, { method: "POST" });
        if (initializeResponse.status == 200) {
            window.location.href = response.source.redirect.url;
        } else {
            console.log("Unable to start iDeal: " + await initializeResponse.text());
            this.setState({ showError: true, error: "We're unable to start your iDeal donation, something is wrong, we're sorry" });
        }
    }

    private renderBankElement(): JSX.Element {
        if (this.props.isRecurring) {
            return (
                <React.Fragment>
                    <div className="alert alert-warning">
                        <i className="fa fa-exclamation-circle" />&nbsp;
                        By providing your IBAN and confirming this payment, you are authorizing Stichting CollAction and Stripe, our payment service provider, to send instructions to your bank to debit your account and your bank to debit your account in accordance with those instructions. 
                        You are entitled to a refund from your bank under the terms and conditions of your agreement with your bank. 
                        A refund must be claimed within 8 weeks starting from the date on which your account was debited.
                    </div>
                    <IbanElement supportedCountries={["SEPA"]} />;
                </React.Fragment>);
        }
        else {
            return <IdealBankElement />;
        }
    }

    render(): JSX.Element {
        return (
            <form onSubmit={(ev) => this.submitPayment(ev)}>
                <div className="error" hidden={!this.state.showError}>
                    <p>{this.state.error}</p>
                </div>
                {this.renderBankElement()}
                <input type="submit" className="btn btn-default" value="submit" />
            </form>);
    }
} 