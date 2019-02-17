import * as React from "react";
import renderComponentIf from "./renderComponentIf";

interface IDonationBoxProps {
}

interface IDonationBoxState {
    amount: number;
    showError: boolean;
}

export default class DonationBox extends React.Component<IDonationBoxProps, IDonationBoxState> {
    constructor(props) {
        super(props);
        this.state = { amount: 0, showError: false };
    }

    setAmount = (event) => {
        let newAmount = Number.parseInt(event.currentTarget.value);
        if (!newAmount) {
            newAmount = 0;
        }
        this.setState({amount: newAmount, showError: false});
    }

    pay = (event) => {
        const option = event.currentTarget.value;
        if (this.state.amount === 0) {
            this.setState({ showError: true});
            return;
        }

        console.log(`Paying eur ${this.state.amount} with payment option ${option}`);
    }

    renderAmount(amount: number) {
        const id = `amount${amount}`;
        return (
            <div className="col-xs-6 col-sm-3">
                <input id={id} type="radio" name="amount" value={amount} onChange={this.setAmount} checked={this.state.amount === amount} />
                <label htmlFor={id}>&euro; {amount}</label>
            </div>
        );
    }

    renderCustomAmount() {
        return (
            <div className="col-xs-6 col-sm-3">
                <input type="text" id="amountCustom"name="amount" placeholder="Other..." onChange={this.setAmount} />
            </div>
        );
    }

    render() {
        return (
            <div className="donation-box">
                <h2>Help us reaching our mission by donating!</h2>
                <p>
                    By donating to CollAction you help us solve collactive action problems and to make the world a better place. Your donation helps us maintaining and growing the platform and ultimately multiply our impact.
                </p>
                <hr />
                <p>
                    If you'd like make a one-time donation, please select the amount and payment option.
                </p>
                <div className="row">
                    {this.renderAmount(3)}
                    {this.renderAmount(5)}
                    {this.renderAmount(10)}
                    {this.renderAmount(20)}
                    {this.renderAmount(30)}
                    {this.renderAmount(50)}
                    {this.renderAmount(100)}
                    {this.renderCustomAmount()}
                </div>
                <div className="error" hidden={!this.state.showError}>
                    <p>Thank you! Please select or enter an amount you'd like to donate.</p>
                </div>
                <div className="payment-options">
                    <div className="row">
                        <div className="col-xs-12">
                            <button className="btn" value="ideal" onClick={this.pay}>
                                <img src="/images/thankyoucommit/iDEAL-logo.png" />
                                <div>iDEAL</div>
                            </button>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-xs-12">
                            <button className="btn" value="creditcard" onClick={this.pay}>
                                <img src="/images/thankyoucommit/bank-card.png" />
                                <div>Debit / Creditcard</div>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

renderComponentIf(
    <DonationBox />,
    document.getElementById("donation-box")
);
