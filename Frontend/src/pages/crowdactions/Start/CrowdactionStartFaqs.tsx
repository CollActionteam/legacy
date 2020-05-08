import React from "react";
import { Faq } from "../../../components/Faq/Faq";

const CrowdactionStartFaqs = () => {
    return <>
      <Faq title="Who can start a crowdaction?" collapsed={true} faqId="who_can_start">
          <p>
              Everyone that adheres to the start crowdaction criteria (see below) can start a
              crowdaction: whether they are individuals, groups of friends or like minded people, or organizations.
          </p>
      </Faq>
      <Faq title="What would be a reasonable target number of participants?" collapsed={true} faqId="number_participants">
          <p>
              Setting the right target for your crowdaction is not easy. It depends on a number of different factors,
              for instance: what type of crowdaction is it? What is the action that is proposed (is it hard or difficult)?
              How much time do you have to promote the crowdaction? Do you have an existing network that you can mobilize?
          </p>
          <p>
              In general, we say: it's better to set a more conservative target and far exceed it, than setting a
              very ambitious target and not reach it by a few percent. If you want to know what a good target would
              be for your crowdaction, a good start would be to think about your communication plan: what are you planning
              to do to reach people? How many people can you reach through each channel that you are planning to use
              (e.g. through your personal network, social media groups, partnering with other organizations/media)?
              Also, please feel free to browse the crowdactions that have already on the Find Crowdaction page, or reach out
              to <a href="mailto:hello@collaction.org">hello@collaction.org</a> - we're happy to help you
              think about this!
          </p>
      </Faq>
      <Faq title="What are the criteria your crowdaction needs to meet?" collapsed={true} faqId="crowdaction_criteria">
          <p>
              A crowdaction can only be listed when it meets the following criteria:
          </p>
          <ul>
              <li>
                  The goal of the crowdaction is to make a positive societal or ecological contribution to your neighbourhood,
                  country or the world.
              </li>
              <li>
                  The crowdaction is not geared towards personal gain.
              </li>
              <li>
                  The crowdaction does not include activities that are focussed on conversion or activism (religious or political).
              </li>
              <li>
                  The crowdaction does not include activities that are illegal or do not abide by the official legislation of the Netherlands or
                  the country of implementation.
              </li>
              <li>
                  The online crowdaction registration form is completed fully and truthfully and has a clear and easily readable crowdaction
                  description and goal. The CollAction team can ask for clarification and/or edit your text if necessary.
              </li>
              <li>
                  The CrowdactionStarter has thought through how people can be moved from commitment to action. We can help you with this!
              </li>
              <li>
                  The crowdaction is ambitious but realistic - The CollAction evaluation commission judges if this is the case.
                  The CrowdactionStarter can activate his/her own network, and/or has a good plan to achieve the target.
              </li>
              <li>
                  The CrowdactionStarter commits to measuring the impact of the action (e.g. the number of people that have acted as
                  a result of the crowdaction) and to sharing this information with CollAction.
              </li>
          </ul>
      </Faq>
      <Faq title="Where can I find more tips and tricks on how to start, run and finish a crowdaction?" collapsed={true} faqId="crowdaction_tips_tricks">
          <p>
              Check out our <a href="https://docs.google.com/document/d/1JK058S_tZXntn3GzFYgiH3LWV5e9qQ0vXmEyV-89Tmw" target="_blank" rel="noopener noreferrer">Crowdaction Starter Handbook</a>
          </p>
      </Faq>
    </>;
}

export default CrowdactionStartFaqs;