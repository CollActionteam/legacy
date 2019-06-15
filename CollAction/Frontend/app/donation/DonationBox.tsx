import renderComponentIf from "../global/renderComponentIf";
import * as React from "react";
import DebitDetailsBox from "./DebitDetailsBox";
import {StripeProvider, Elements, injectStripe} from "react-stripe-elements";
import * as Modal from "react-modal";

const InjectedDebitDetailsBox = injectStripe(DebitDetailsBox);

const popupStyles = {
  content : {
    top                   : "50%",
    left                  : "50%",
    right                 : "auto",
    bottom                : "auto",
    marginRight           : "-50%",
    transform             : "translate(-50%, -50%)",
    backgroundColor       : "#efefef",
    overlay: {
       backgroundColor: "#efefef"
    }
  }
};

interface IDonationBoxProps {
    stripePublicKey: string;
    userEmail: string;
    userName: string;
}

interface IDonationBoxState {
    amount: number;
    showError: boolean;
    error: string;
    showBankDialog: boolean;
    inputUserName: string;
    inputUserEmail: string;
    isRecurring: boolean;
}

class DonationBox extends React.Component<IDonationBoxProps, IDonationBoxState> {
    constructor(props) {
        super(props);
        this.state = {
            amount: 0,
            showError: false,
            error: "",
            showBankDialog: false,
            isRecurring: false,
            inputUserEmail: "",
            inputUserName: ""
        };
    }

    isValidEmail(email: string): boolean {
        return email.match(/^\S+@\S+\.\S+$/) !== null;
    }

    clearErrors(): void {
        this.setState({ showError: false, error: null });
    }

    needsToEnterName(): boolean {
        return this.props.userName.trim() === "";
    }

    needsToEnterEmail(): boolean {
        return !this.isValidEmail(this.props.userEmail);
    }

    checkFormErrors(): boolean {
        this.clearErrors();
        if (this.state.amount <= 0) {
            this.setState({ showError: true, error: "Please select a donation amount" });
            return true;
        }

        if (this.getName() === "") {
            this.setState({ showError: true, error: "Please provide your name" });
            return true;
        }

        if (!this.isValidEmail(this.getEmail())) {
            this.setState({ showError: true, error: "Please provide a valid e-mail address" });
            return true;
        }

        return false;
    }

    payDebit(): void {
        if (this.checkFormErrors()) {
            return;
        }
        this.setState({ showBankDialog: true });
    }

    async payCreditCard(): Promise<void> {
        if (this.checkFormErrors()) {
            return;
        }

        let checkoutTokenUrl = `/Donation/InitializeCreditCardCheckout?currency=eur&amount=${this.state.amount}&name=${encodeURIComponent(this.getName())}&email=${encodeURIComponent(this.getEmail())}&recurring=${this.state.isRecurring}`;
        let checkoutTokenResponse: Response = await fetch(checkoutTokenUrl, { method: "POST" });
        if (checkoutTokenResponse.status !== 200) {
            let responseBody = await checkoutTokenResponse.text();
            console.log("Unable to redirect to checkout: " + responseBody);
            this.setState({ showError: true, error: "We're unable to start your credit-card donation, there is something wrong, sorry. "});
            return;
        }

        let stripe: any = Stripe(this.props.stripePublicKey); // cast to any because redirectToCheckout is not yet in stripe.js
        let checkoutId = await checkoutTokenResponse.text();
        let checkoutResponse = await stripe.redirectToCheckout({ sessionId: checkoutId });
        if (checkoutResponse.status !== 200) {
            let responseBody = await checkoutResponse.text();
            console.log("Unable to redirect to checkout: " + responseBody);
            this.setState({ showError: true, error: "We're unable to start your credit-card donation, there is something wrong, sorry. "});
        }
    }

    onCloseDialog(): void {
        this.setState({ showBankDialog: false });
    }

    setName(ev: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ inputUserName: ev.currentTarget.value });
    }

    setEmail(ev: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ inputUserEmail: ev.currentTarget.value });
    }

    setAmount(event: React.ChangeEvent<HTMLInputElement>): void {
        let newAmount = Number.parseInt(event.currentTarget.value);
        if (!newAmount) {
            newAmount = 0;
        }
        this.setState({amount: newAmount, showError: false});
    }

    setIsOneOff(event: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ isRecurring: !event.currentTarget.checked });
    }

    setIsMonthly(event: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ isRecurring: event.currentTarget.checked });
    }

    getName(): string {
        if (!this.needsToEnterName()) {
            return this.props.userName.trim();
        } else {
            return this.state.inputUserName.trim();
        }
    }

    getEmail(): string {
        if (!this.needsToEnterEmail()) {
            return this.props.userEmail.trim();
        } else {
            return this.state.inputUserEmail.trim();
        }
    }

    private renderPeriodSelection(): JSX.Element {
        return (
            <div className="row payment-selection-options">
                <div className="col-xs-6">
                    <input id="one-off-donation-button" type="radio" name="period" value="one-off" onChange={(event) => this.setIsOneOff(event)} checked={!this.state.isRecurring} />
                    <label htmlFor="one-off-donation-button">One-off</label>
                </div>
                <div className="col-xs-6">
                    <input id="monthly-donation-button" type="radio" name="period" value="one-off" onChange={(event) => this.setIsMonthly(event)} checked={!!this.state.isRecurring} />
                    <label htmlFor="monthly-donation-button">Monthly</label>
                </div>
            </div>
        );
    }

    private renderAmount(amount: number): JSX.Element {
        const id = `amount${amount}`;
        return (
            <div className="col-xs-6 col-sm-3">
                <input id={id} type="radio" name="amount" value={amount} onChange={(event) => this.setAmount(event)} checked={this.state.amount === amount} />
                <label htmlFor={id}>&euro; {amount}</label>
            </div>
        );
    }

    private renderCustomAmount(): JSX.Element {
        return (
            <div className="col-xs-6 col-sm-3">
                <input type="text" id="amountCustom" name="amount" placeholder="Other..." onChange={(event) => this.setAmount(event)} />
            </div>
        );
    }

    private renderName(): JSX.Element {
        if (this.needsToEnterName()) {
            return (
                <div className="col-xs-12">
                    <label htmlFor="name-input">Name</label>
                    <input id="name-input" className="form-control" onChange={(ev) => this.setName(ev)} placeholder="Your name..." type="text" required />
                </div>
            );
        }
    }

    private renderEmail(): JSX.Element {
        if (this.needsToEnterEmail()) {
            return (
                <div className="col-xs-12">
                    <label htmlFor="email-input">E-Mail</label>
                    <input id="email-input" className="form-control" onChange={(ev) => this.setEmail(ev)} placeholder="Your e-mail..." type="email" required />
                </div>
            );
        }
    }

    private renderUserDetails(): JSX.Element {
        if (this.needsToEnterEmail() || this.needsToEnterName()) {
            return (
                <div className="row user-details">
                    {this.renderName()}
                    {this.renderEmail()}
                </div>
            );
        }
    }

    private renderAmounts(): JSX.Element {
        return (
            <div className="row payment-selection-options">
                {this.renderAmount(3)}
                {this.renderAmount(5)}
                {this.renderAmount(10)}
                {this.renderAmount(20)}
                {this.renderAmount(30)}
                {this.renderAmount(50)}
                {this.renderAmount(100)}
                {this.renderCustomAmount()}
            </div>);
    }

    private detailsBox: DebitDetailsBox;

    private renderPopup(): JSX.Element {
        return (
            <Modal isOpen={this.state.showBankDialog} onRequestClose={() => this.onCloseDialog()} contentLabel="Donate" style={popupStyles}>
                <div id="donation-modal">
                    <StripeProvider apiKey={this.props.stripePublicKey}>
                        <Elements>
                            <InjectedDebitDetailsBox
                                onRef={ref => (this.detailsBox = ref)}
                                amount={this.state.amount}
                                userEmail={this.getEmail()}
                                userName={this.getName()}
                                isRecurring={this.state.isRecurring}
                            />
                        </Elements>
                    </StripeProvider>
                    <div className="buttons">
                        <input type="button" className="btn btn-default" value="Donate!" onClick={() => this.detailsBox.submitPayment()}></input>
                        <input type="button" className="btn" value="Close" onClick={() =>  this.onCloseDialog()}></input>
                    </div>
                </div>
            </Modal>);
    }

    private renderPaymentOptions(): JSX.Element {
        return (<div className="payment-options">
            <div className="row">
                <div className="col-xs-12">
                    <button className="btn" value="ideal" onClick={() => this.payDebit()}>
                        <img src="/images/thankyoucommit/iDEAL-logo.png" />
                        <div>Debit (iDeal / SEPA Direct)</div>
                    </button>
                </div>
            </div>
            <div className="row">
                <div className="col-xs-12">
                    <button className="btn" value="creditcard" onClick={() => this.payCreditCard()}>
                        <img src="/images/thankyoucommit/bank-card.png" />
                        <div>Creditcard</div>
                    </button>
                </div>
            </div>
        </div>);
    }

    private renderErrors(): JSX.Element {
        return (
            <div className="error" hidden={!this.state.showError}>
                <p>{this.state.error}</p>
            </div>);
    }

    private renderHeader(): JSX.Element {
        return (
            <div>
                <h2>Help us reach our mission by donating!</h2>
                <p>
                    CollAction aims to move millions of people to act for good. We're a small team of
                    passionate volunteers and we keep cost super low - but some costs are involved in
                    maintaining and growing the platform. With your support we can grow this crowdacting
                    movement and safeguard our independence. Many thanks for your help!
                </p>
                <hr />
            </div>);
    }

    render(): JSX.Element {
        return (
            <div className="donation-box">
                {this.renderHeader()}
                {this.renderUserDetails()}
                {this.renderErrors()}
                {this.renderPeriodSelection()}
                {this.renderAmounts()}
                {this.renderPopup()}
                {this.renderPaymentOptions()}
            </div>
        );
    }
}

Modal.setAppElement(document.querySelector("#main"));

let donationBox: HTMLElement = document.getElementById("donation-box");
if (donationBox !== null) {
    let stripePublicKey = donationBox.getAttribute("data-stripe-public-key");
    let name = donationBox.getAttribute("data-user-name");
    let email = donationBox.getAttribute("data-user-email");
    renderComponentIf(
        <DonationBox stripePublicKey={stripePublicKey} userName={name} userEmail={email} />,
        donationBox
    );
}