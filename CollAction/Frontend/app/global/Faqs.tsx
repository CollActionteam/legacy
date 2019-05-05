import * as React from "react";
import Collapsible from "react-collapsible";

import renderComponentIf from "./renderComponentIf";

interface IFaqProps {
    title: any;
    content: any;
    open?: string;
}

interface IFaqState {
    open: boolean;
}

export default class Faq extends React.Component<IFaqProps, IFaqState> {
    constructor(props: IFaqProps) {
        super(props);

        this.state = {
            open: this.props.open === "true" ? true : false
        };
    }

    renderTitle() {
        return (
            <h3>
                <a href="#" className="faq-title">{this.props.title}</a>
                <span className="toggle">
                    <i className="fa fa-plus"></i>
                    <i className="fa fa-minus"></i>
                </span>
            </h3>
        );
    }

    render() {
        return (
            <div className="faq-item">
                <Collapsible open={this.state.open} trigger={this.renderTitle()} transitionTime={300} lazyRender={true}>
                    {this.props.content}
                </Collapsible>
            </div>
        );
    }
}

renderComponentIf(
    <Faq
        title="What is CollAction?"
        content={
            <span>
                <p>
                    CollAction is a not-for-profit organization based in the Netherlands that
                    helps people to solve Collective Action Problems through crowdacting.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-what-is-collaction")}
    />,
    document.getElementById("faq-what-is-collaction")
);

renderComponentIf(
    <Faq
        title="Huh? What is crowdacting?"
        content={
            <span>
                <p>
                    The term Crowdacting, as it is used here, is a term that has been introduced by CollAction.
                    Basically, it means: ‘taking action collectively and conditionally with the purpose of achieving
                    positive impact on the world and its inhabitants’. Simply put, with crowdacting we ask:
                    ‘Would you take action if you knew that a hundred/a thousand/a million/a billion others would do so too?’
                    An example of a crowdacting project: ‘If we can find 10,000 people that commit to switch to renewable
                    energy if 9,999 others would do the same, we will all switch together’. As a result, we form a
                    critical mass of people that has a huge impact on making this world a better place. The concept is very
                    similar to crowdfunding, but people commit to actions, instead of money.
                </p>
                <p>
                In practice, a crowdacting project follows three steps:
                </p>
                <ol>
                    <li>The ProjectStarter launches a project on collaction.org. He/she proposes a certain action
                        (for example switching to renewable energy), a target (minimum number of participants), and a deadline.</li>
                    <li>ProjectSupporters commit to taking action, if the target is met within the deadline.</li>
                    <li>If and when the target is met, everybody acts collectively.</li>
                </ol>
                <p>
                    More information can be found on <a href="http://www.crowdacting.org/" target="_blank">www.crowdacting.org</a>.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-what-is-crowdacting")}
    />,
    document.getElementById("faq-what-is-crowdacting")
);

renderComponentIf(
    <Faq
        title="Crowdacting – is that a new thing?"
        content={
            <span>
                <p>
                    Although the term crowdacting in its current meaning is new, the underlying concept is not.
                    There are plenty of other examples of collective actions that contain elements of crowdacting.
                    Think for instance of boycotts, demonstrations, collective bargaining initiatives, or petition websites.
                    The difference is the fact that crowdacting combines three elements: 1) It’s (explicitly)
                    conditional (it only happens if a set target is met); 2) it’s for social and/or ecological good; and
                    3) the action goes beyond signing a petition. More information on how crowdacting resembles and differs
                    from other initiatives can be found on <a href="http://www.crowdacting.org/" target="_blank">www.crowdacting.org</a>.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-crowdacting-a-new-thing")}
    />,
    document.getElementById("faq-crowdacting-a-new-thing")
);

renderComponentIf(
    <Faq
        title="What are collective action problems?"
        content={
            <span>
                <p>
                    Collective action problems are situations where it is in the interest of the group to work together, but
                    in which coordination is a challenge. Because of this, every individual has an incentive to act in their
                    own interest. This <a href="https://www.youtube.com/watch?v=p3KlgxYhDbk" target="_blank">video</a> explains
                    it clearly.
                </p>
                <p>
                    A well-known example of a collective action problem is the tragedy of the commons.
                    The commons is a communal meadow where cows of different farmers can graze. Every farmer wants to earn as much
                    as possible against the lowest possible costs and therefore has the individual incentive to let as many of
                    his cows graze on the communal grass as possible. However, other farmers will do the same and soon the meadow will
                    be destroyed due to overgrazing. There is little point in doing something about this for each individual farmer
                    – knowing (or fearing) that other farmers will try and use the largest possible share of the communal grass and
                    the meadow will still be overgrazed. The farmer is better off adding some more cows, so he will, at least, benefit
                    from it in the short term. However, in the long term, all the farmers and as a consequence the community will lose out,
                    because the piece of land can be written off due to overgrazing.
                </p>
                <p>
                    Examples of (serious and less serious) collective action problems are: global warming, traffic jams, overpopulation,
                    dismal working conditions, keeping a student house clean, corruption, overfishing, etc.
                </p>
                <p>
                    Traditionally, there are two solutions to collective action problems:
                </p>
                <ol>
                    <li>Privatisation: the commons can be divided between the farmers. Now the farmers benefit from taking good care of
                        their own piece of land, so that it is of use to them in the long term.</li>
                    <li>Regulation: Agreements can be made and enforced about the usage of the commons. It is, for example, possible to
                        decide on a maximum number of cows per farmer.</li>
                </ol>
                <p>
                    However, these solutions are not applicable to all collective action problems. It is not so easy (or desirable) to
                    privatise certain goods (how do you privatise the ozone layer? Or migrating fish populations?). Coming up with
                    regulations and enforce these regulations to solve (cross border) problems has proven difficult - just think about
                    the cumbersome process to do something about climate change on an international level.
                </p>
                <p>
                    So, we need a new solution to old problems: crowdacting.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-what-are-collactive-problems")}
    />,
    document.getElementById("faq-what-are-collactive-problems")
);

renderComponentIf(
    <Faq
        title="Where can I learn more about crowdacting?"
        content={
            <span>
                <a href="http://www.crowdacting.org/" target="_blank">www.crowdacting.org</a>.
            </span>
        }
        open={getOpenAttribute("faq-where-to-learn-more")}
    />,
    document.getElementById("faq-where-to-learn-more")
);

renderComponentIf(
    <Faq
        title="Can I start a project myself?"
        content={
            <span>
                Yes! You can create a project <a href="/Projects/StartInfo" target="_blank">here</a>. Make sure to check if your idea
                meets the CollAction criteria below.
            </span>
        }
        open={getOpenAttribute("faq-where-to-learn-more")}
    />,
    document.getElementById("faq-start-it-yourself")
);

renderComponentIf(
    <Faq
        title="How long does a project run for?"
        content={
            <span>
                Some actions are a one-off – think of the switch to a fair bank or green energy. Other projects can have a longer
                duration, such as eating less meat, periodically visiting lonely elderly people or helping refugees with language
                lessons and their integration. In other words, the duration of an action differs from one project to the other.
            </span>
        }
        open={getOpenAttribute("faq-how-long-does-it-run")}
    />,
    document.getElementById("faq-how-long-does-it-run")
);

renderComponentIf(
    <Faq
        title="What happens when a target has not been reached?"
        content={
            <span>
                If the target is not met by the deadline, no one needs to act. Crowdacting means: No cure, no action. However,
                if you are really inspired to take action anyway, we will of course applaud this.
            </span>
        }
        open={getOpenAttribute("faq-how-long-does-it-run")}
    />,
    document.getElementById("faq-target-not-reached")
);

renderComponentIf(
    <Faq
        title="How do I know that other people will really take action?"
        content={
            <span>
                <p>
                    You can never be 100% sure. In the first place, crowdacting is about trust. Trusting that people want to work
                    together to create a better world. Trusting that people are attached to their personal ideals. And trusting that
                    people will keep their promises. We hope to build a community of people that take their commitments seriously and
                    try to create a culture in which agreement=agreement.
                </p>
                <p>
                    That being said, we will incorporate certain processes and tools to ensure that as many project supporters
                    proceed to taking action. What these processes are depends on the type of project. For some projects
                    (such as switching projects, for example to a fair bank or green energy) the action can already be started
                    conditionally at the time of commitment. When the target is reached, supporters of the project will automatically
                    be transferred to the fair bank or renewable energy provider. When it comes to local projects, social control will
                    play a bigger role. Other projects might be purely based on trust. We will be testing different tools and approaches
                    in the upcoming projects. We ask ProjectStarters to seriously consider this question  – the team of CollAction can
                    help you think this through!
                </p>
            </span>
        }
        open={getOpenAttribute("faq-how-do-you-know")}
    />,
    document.getElementById("faq-how-do-you-know")
);

renderComponentIf(
    <Faq
        title="When will the projects end exactly?"
        content={
            <span>
                <p>
                    The project ends at the end (23:59:59) of the day that is listed as "Sign up closes" in the UTC (London) timezone.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-when-will-it-end")}
    />,
    document.getElementById("faq-when-will-it-end")
);

renderComponentIf(
    <Faq
        title="Where can I learn more about the CollAction organization?"
        content={
            <span>
                <p>
                    You can read more about our mission, strategy, and financials in <a href="https://drive.google.com/open?id=1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey">this document</a>.
                    The document is in Dutch for now, since we are headquartered in the Netherlands and need to submit it to local
                    authorities - but we're working on a translation - apologies!
                    But if you paste it in Google Translate, you should be able to get the gist ☺.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-about-the-organization")}
    />,
    document.getElementById("faq-about-the-organization")
);

renderComponentIf(
    <Faq
        title="Who can start a project?"
        content={
            <span>
                <p>
                    Everyone that adheres to the start project criteria (see below) can start a
                    project: whether they are individuals, groups of friends or like minded people, or organizations.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-who-can-start")}
    />,
    document.getElementById("faq-who-can-start")
);

renderComponentIf(
    <Faq
        title="What would be a reasonable target number of participants?"
        content={
            <span>
                <p>
                    Setting the right target for your project is not easy. It depends on a number of different factors,
                    for instance: what type of project is it? What is the action that is proposed (is it hard or difficult)?
                    How much time do you have to promote the project? Do you have an existing network that you can mobilize?
                </p>
                <p>
                    In general, we say: it's better to set a more conservative target and far exceed it, than setting a
                    very ambitious target and not reach it by a few percent. If you want to know what a good target would
                    be for your project, a good start would be to think about your communication plan: what are you planning
                    to do to reach people? How many people can you reach through each channel that you are planning to use
                    (e.g. through your personal network, social media groups, partnering with other organizations/media)?
                    Also, please feel free to browse the projects that have already on the Find Project page, or reach out
                    to <a href="mailto:collactionteam@gmail.com">collactionteam@gmail.com</a> - we're happy to help you
                    think about this!
                </p>
            </span>
        }
        open={getOpenAttribute("faq-reasonable-target")}
    />,
    document.getElementById("faq-reasonable-target")
);

renderComponentIf(
    <Faq
        title="What are the criteria your project needs to meet?"
        content={
            <span>
                <p>
                    A project can only be listed when it meets the following criteria:
                </p>
                <ul>
                    <li>
                        The goal of the project is to make a positive societal or ecological contribution to your neighbourhood,
                        country or the world.
                    </li>
                    <li>
                        The project is not geared towards personal gain.
                    </li>
                    <li>
                        The project does not include activities that are focussed on conversion or activism (religious or political).
                    </li>
                    <li>
                        The project does not include activities that are illegal or do not abide by the official legislation of the Netherlands or
                        the country of implementation.
                    </li>
                    <li>
                        The online project registration form is completed fully and truthfully and has a clear and easily readable project
                        description and goal. The CollAction team can ask for clarification and/or edit your text if necessary.
                    </li>
                    <li>
                        The ProjectStarter has thought through how people can be moved from commitment to action. We can help you with this!
                    </li>
                    <li>
                        The project is ambitious but realistic - The CollAction evaluation commission judges if this is the case.
                        The ProjectStarter can activate his/her own network, and/or has a good plan to achieve the target.
                    </li>
                    <li>
                        The ProjectStarter commits to measuring the impact of the action (e.g. the number of people that have acted as
                        a result of the project) and to sharing this information with CollAction.
                    </li>
                </ul>
            </span>
        }
        open={getOpenAttribute("faq-criteria")}
    />,
    document.getElementById("faq-criteria")
);

renderComponentIf(
    <Faq
        title="Where can I find more tips and tricks on how to start, run and finish a project?"
        content={
            <span>
                <p>
                    Check out our <a href="https://docs.google.com/document/d/1JK058S_tZXntn3GzFYgiH3LWV5e9qQ0vXmEyV-89Tmw" target="_blank">Project Starter Handbook</a>
                </p>
            </span>
        }
        open={getOpenAttribute("faq-tips-and-tricks")}
    />,
    document.getElementById("faq-tips-and-tricks")
);

renderComponentIf(
    <Faq
        title="Why should I donate?"
        content={
            <ul>
                <li>
                    Our goal is to move millions of people to act for good by launching the crowdacting movement. Whereas back in the day,
                    people turned to politicians and policy makers to fix the world's social and environmental problems (with different levels
                    of success :) ), we think it's time for a new approach. With crowdacting, we take matters into our own collective hands.
                </li>
                <li>
                    But in order to reach our ambitious goals, we need you support. CollAction is a non profit organization. We keep costs
                    super low with the support of our amazing team of volunteers and great pro bono supporters. However, there are still
                    certain costs that need to be covered (you can find an overview of our financials <a href="https://drive.google.com/file/d/1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey/view">here</a>).
                    We don't like to be dependent on subsidies and we believe it is vital to remain independent from commercial interests.
                    Hence, we ask for contributions from the crowd to survive, scale our impact, and remain independent.
                </li>
            </ul>
        }
        open={getOpenAttribute("faq-why-donate")}
    />,
    document.getElementById("faq-why-donate")
);

renderComponentIf(
    <Faq
        title="What will my donation be spent on? "
        content={
            <span>
                <p>
                    You can find an overview of our financials <a href="https://drive.google.com/file/d/1syiUgEq-Or-GOfvBYY8k3fgTu5UZvLey/view">here</a> (apologies, it's in Dutch, since that's where our headquarter is based). 
                    In short, the main part of our budget goes to website and
                    organizational costs (e.g. hosting, membership at a co-working space, banking costs). There are also costs related to events, but we
                    generally break even on these events, e.g. by selling tickets, so they pay for themselves. As a team of volunteers, we manage to do
                    a lot with just a little - your donation will go a long way. Our ambitious goal is to start the crowdacting movement and inspire
                    millions of people to act for good by the end of 2020. All money is spent towards that goal.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-donation-goes-to")}
    />,
    document.getElementById("faq-donation-goes-to")
);

renderComponentIf(
    <Faq
        title="Who started CollAction?"
        content={
            <span>
                <p>
                    CollAction is started by a <a href="/about">team</a> of optimistic and pragmatic people that believe we can make this world a
                    better place through crowdacting. The concept of CollAction/crowdacting was born in The Netherlands, but we now have an
                    international team of around 20 volunteers.
                </p>
            </span>
        }
        open={getOpenAttribute("faq-collaction-started")}
    />,
    document.getElementById("faq-collaction-started")
);

function getOpenAttribute(id) {
    return document.getElementById(id) && document.getElementById(id).dataset.open;
}
