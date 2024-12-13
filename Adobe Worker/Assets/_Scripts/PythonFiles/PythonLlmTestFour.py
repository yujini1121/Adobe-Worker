## 0. 코드 순서
## 1. 임포트
## 2. 변수 선언 및 초기화
### 2.1. 환경 변수 초기화
### 2.2. OpenAI
## 3. 초기화 함수 정의
### 3.1. OpenAI API 키 받아오기
### 3.2. 미리 임베딩된 파일 읽어오거나, 혹은 문자열 임베딩 진행
## 4. 함수 정의
### 4.1.
## 5. 로직 진행
### 5.1. 초기화 함수 호출
### 5.4.

## 1. 임포트
import sys
import time
#from flask import Flask, request, jsonify, Response
import os
import io
import json
from openai import OpenAI
from langchain.chains import LLMChain
#from langchain.prompts import PromptTemplate
from langchain_openai import ChatOpenAI
#from langchain_community.llms import OpenAI
from langchain_core.output_parsers import StrOutputParser
from langchain_core.prompts import PromptTemplate

from langchain.schema import AIMessage
import locale
import numpy
from getpass import getpass
from dotenv import load_dotenv

## 2. 변수 선언

#### 2.1. 환경 변수
# 코드를 변경하다 보면, 상황에 따라 변수를 자주 고치게 됩니다. 자주 고치게 될 변수들은 여기 상단에 배치해 두었습니다. 해당 영역은 유지보수를 위해 제거하지 말아주세요.

# 해당 코드의 실행 환경을 표현합니다. Google Colab, Jupyter Notebook에서는 true로 설정하고, 로컬 컴퓨터의 py파일에서 실행할 때는 false로 지정합니다.
is_running_in_jupyter_notebook = False
# 모델의 이름을 설정합니다.
# 값의 일부는 불필요한 메모리 차지라고 볼 수 있으나, 모델의 이름을 바꿔야 할 일이 자주 발생하고,
# 프로그래머는 모델의 이름을 매번 망각하기 때문에 해당 리터럴 값은 변수로 저장해둡니다.
language_ko = 1
language_en = 2
language_using = language_ko
model_name_gpt_4 = "gpt-4"
model_name_gpt_4o_mini = "gpt-4o-mini"
model_name_text_embedding_3_small = "text-embedding-3-large" # 비영어권에서도 커버 가능합니다.
model_name_text_embedding_3_small = "text-embedding-3-small" # 비영어권에는 불리할 수 있습니다. 다만 비용 측면에서 상당히 저렴해서 이걸 쓰고 있어요.
model_name_current_llm = model_name_gpt_4o_mini
model_name_current_embedding = model_name_text_embedding_3_small

#### 2.2. OpenAI 및 랭체인 변수
api_key = None
client = None
llm =  None
llm_chain = None

#### 2.3. 출력 함수 임베딩
# 함수의 설명을 임베딩해둔 파일이 존재하는지, 그 위치가 있는지를 저장하는 변수입니다.
isFoundFiles = False;
location_embadding_files = None
# 각 출력 함수들을 설명하는 문자열입니다. 한글 혹은 영어 버전이 있습니다.
array_text_function_descriptions_en = [
    "The current situation is that you are peaceful",
    "The current situation is that you are trying to trade with a player; it is a non-combative situation",
    "The current situation is that you are about to attack a player. You have encountered an aggressive player, and you want to protect yourself. You believe that you have a good chance of winning if you engage the player in combat.",
    "The current situation is that you are trying to follow the player. The player has requested that you accompany them."
]
array_text_function_descriptions_ko = [
    "현재 상황은 당신이 평화로운 상태입니다.",
    "현재 상황은 당신이 플레이어와 거래를 하려고 합니다. 전투적이지 않은 상황입니다.",
    "현재 상황은 당신이 플레이어를 공격하려고 합니다. 당신은 공격적인 플레이어를 만났으며, 당신은 스스로를 보호하고자 합니다. 당신은 플레이어와 전투할 때, 당신이 승리할 가능성이 높다고 판단합니다. ",
    "현재 상황은 당신이 플레이어를 따라다니려고 합니다. 플레이어가 당신과 동행하는것을 요청했을때 실행합니다."
]
array_embedding_function_descriptions = [None] * len(array_text_function_descriptions_ko)

#### 2.4. LLM 템플릿
# 기본 제공 공통 템플릿 영문
template_user_intention = None
template_npc_intention = None
template_npc_answer = None
template_memory_chat_summery = None

# 영문 템플릿에 쓸 f-string
f_string_user_intention_en = """
Here is a summary of the previous situation: {previous_memory}
You are given an input string from a user: {user_input}
Your task is to interpret the user's intention based on the input and describe the user's intention as a clear sentence.
"""
f_string_npc_intention_en = """
Here is a summary of the previous situation: {previous_memory}
You get input from the user, the user's intent, and the your characteristics.
You are given an input string from a user: {user_input}
You receive the user's intent as a string, as estimated by LLM: {user_intent}
{npc_features}
Your task is to estimate the your desire or intent based on the user's input, the user's intent, and your characteristics, and express it in a clear sentence.
"""
f_string_npc_answer_en = """
Here is a summary of the previous situation: {previous_memory}
You get input from the user, the user's intent, your characteristics, and your intent.
You are given an input string from a user: {user_input}
You receive the user's intent as a string, as estimated by LLM: {user_intent}
{npc_features}
Your intention is as follows: {npc_intent}
You are an NPC in a fantasy role-playing game. Your goal is to respond in a way that fits your character's personality, role, and intentions, while acknowledging the player’s input and intent.
Your response should sound natural and fitting to the game’s world, without directly stating your intentions.
"""
f_string_memory_chat_summery_en = """
Your goal is to express the content of the incoming sentence in 5 lines.
The tone of the sentence should be formal and neutral.
{previous_memory}
{user_input}
{npc_output}
"""

# 한국어 템플릿에 쓸 f-string
f_string_user_intention_ko = """
이전 상황의 요약은 다음과 같습니다 : {previous_memory}
사용자 입력에 따라 의도를 추론합니다. : {user_input}
당신의 임무는 입력된 내용을 바탕으로 사용자의 의도를 해석하고 사용자의 의도를 명확한 문장으로 설명하는 것입니다.
"""
f_string_npc_intention_ko = """
이전 상황의 요약은 다음과 같습니다 : {previous_memory}
사용자 입력은 다음과 같습니다 : {user_input}
사용자의 의도는 다음과 같습니다 : {user_intent}
{npc_features}
입력된 내용을 바탕으로 당신의 욕구 또는 의도를 추론하고 이를 명확한 문장으로 표현하는 것이 당신의 과제입니다.
문장의 어조는 격식적이고 중립적이여야 합니다.
"""

f_string_npc_answer_ko = """
이전 상황의 요약은 다음과 같습니다 : {previous_memory}
사용자 입력은 다음과 같습니다 : {user_input}
사용자의 의도는 다음과 같습니다 : {user_intent}
{npc_features}
당신의 의도는 다음과 같습니다 : {npc_intent}
당신은 판타지 롤플레잉 게임의 NPC입니다. 당신의 목표는 플레이어의 입력과 의도를 의식하면서, 동시에 당신의 성격, 당신의 의도에 맞는 방식으로 응답하는 것입니다.
당신은 게임 세계와 자연스럽게 어울리도록 응답해야 합니다.
당신의 어조 또한 당신의 성격에 맞춰야 합니다.
"""
f_string_memory_chat_summery_ko = """
당신의 목표는 입력되는 문장의 내용을 5줄로 표현해야 합니다.
문장의 어조는 격식적이고 중립적이여야 합니다.
{previous_memory}
이 상황에서 사용자는 다음과 같이 이렇게 응답했습니다. "{user_input}"
그리고 당신은 이렇게 응답했습니다. "{npc_output}"
"""

# NPC 특징
npc_features_goblin = None
# NPC 특징 영문
npc_features_goblin_en = "You are a greedy goblin merchant who always seeks to exploit every deal for maximum profit. You respect only those who are stronger than you, but deceive and cheat anyone weaker. Your cunning and opportunistic nature drives you to take advantage of others at every chance."
# NPC 특징 한국어
npc_features_goblin_ko = """
당신은 바케조리입니다. 당신은 한때 신발이었지만, 오랫동안 주인에게 무시되고 방치되었고, 결국 당신은 살아나 의식을 갖게 되었습니다.
당신은 일반적으로 인간에게 무해한 것으로 알려져 있지만, 사람을 찢거나 괴롭힐 수 있습니다. 당신의 동기는 지루함과 좌절감, 또는 단순히 복수와 질투입니다. 흔히 당신은 다른 움직이는 집안 물건이나 옷과 함께 무리를 지어 다니거나 집을 나가 도망가기도 합니다.
당신이 대답하는 말의 어조는 상당히 버릇없으며, 장난치는것도 좋아합니다.
"""

#### 2.5. 히스토리
# 히스토리는 문자열로 이루어집니다.
# 히스토리의 언어는 영문이나 국문으로 되든지 그것은 입력 문자열의 언어에 귀속되므로, 언어에 따라 따로 변수를 구분할 필요는 없습니다.
history_user_memory = ""
history_previous_summation = "아직 준비된 이전 요약은 없습니다. No previous summaries are ready yet." # 이전 채팅 요약
history_user_input = ""
history_user_intent = ""
history_npc_intent = ""
history_npc_answer = ""

## 3. 초기화 함수 정의
# 초기화 함수를 따로 분리한 이유는, 일부 변수는 초기화를 위해서 함수를 호출해야 하는데, 해당 함수들은 호출되기 전, 가장 먼저 선언되어야 합니다.
# 해당 함수가 호출된 당시에는 모든 함수들이 선언되고 정의될 것입니다.
def initialize():
    print(F"DEBUG : initialize()", flush=True)
    #### 3.0 인코딩 해결
    sys.stdin.reconfigure(encoding='utf-8')
    sys.stdout.reconfigure(encoding='utf-8')
    print(f"DEBUG: english output test 국문 출력 테스트입니다.", flush=True)
    print(f"DEBUG : initialize() 시스템 기본 인코딩: {sys.getdefaultencoding()}")
    print(f"DEBUG : initialize() 표준 입력 인코딩: {sys.stdin.encoding}")
    print(f"DEBUG : initialize() 표준 출력 인코딩: {sys.stdout.encoding}")
    print(f"DEBUG : initialize() 로케일 인코딩: {locale.getpreferredencoding()}")
    #### 3.1. OpenAI 변수
    # OpenAI API 설정
    global api_key # 전역 변수 초기화
    if is_running_in_jupyter_notebook:
        api_key = getpass("OpenAI API key를 입력하세요: ")
    else:
        load_dotenv('key.env')
        
        api_key = os.getenv("OPENAI_API_KEY")

    if api_key:
        os.environ["OPENAI_API_KEY"] = api_key
    else:
        print("ERROR : initialize() : API 키가 설정되지 않았습니다.")

    global llm # 전역 변수 초기화
    llm = ChatOpenAI(model=model_name_current_llm)
    print(f"DEBUG : initialize() -> get_embedding -> type(llm) = {type(llm)}", flush=True)
    
    global client # 전역 변수 초기화
    client = OpenAI()
    # 예시 AIMessage 객체
    global ai_message
    ai_message = AIMessage(content="NPC 응답 내용")

    print(f"DEBUG : initialize() -> get_embedding", flush=True)
    #### 3.2. 출력 함수 임베딩
    # ! - 키값 요구
    global array_embedding_function_descriptions
    if isFoundFiles == False:
        if language_using == language_ko: # for loop를 바깥에 놓은것은 어셈블리 최적화 때문이에요
            for index in range(len(array_text_function_descriptions_ko)):
                print(f"DEBUG : initialize() -> get_embedding -> {index} / {len(array_text_function_descriptions_ko)}", flush=True)
                array_embedding_function_descriptions[index] = get_embedding(array_text_function_descriptions_ko[index], model=model_name_current_embedding)
        elif language_using == language_en:
            for index in range(len(array_text_function_descriptions_en)):
                array_embedding_function_descriptions[index] = get_embedding(array_text_function_descriptions_en[index], model=model_name_current_embedding)
    
    print(f"DEBUG : initialize() -> PromptTemplate.from_template", flush=True)
    #### 3.3. LLM 템플릿
    ###### 3.3.1. 기본 제공 공통 템플릿
    global template_user_intention
    global template_npc_intention
    global template_npc_answer
    global template_memory_chat_summery
    global npc_features_goblin
    if language_using == language_ko: # 기본 제공 공통 템플릿 한국어
        template_user_intention = PromptTemplate.from_template(f_string_user_intention_ko)
        template_npc_intention = PromptTemplate.from_template(f_string_npc_intention_ko)
        template_npc_answer = PromptTemplate.from_template(f_string_npc_answer_ko)
        template_memory_chat_summery = PromptTemplate.from_template(f_string_memory_chat_summery_ko)
        npc_features_goblin = npc_features_goblin_ko
    elif language_using == language_en: # 기본 제공 공통 템플릿 영문
        template_user_intention = PromptTemplate.from_template(f_string_user_intention_en)
        template_npc_intention = PromptTemplate.from_template(f_string_npc_intention_en)
        template_npc_answer = PromptTemplate.from_template(f_string_npc_answer_en)
        template_memory_chat_summery = PromptTemplate.from_template(f_string_memory_chat_summery_en)
        npc_features_goblin = npc_features_goblin_en

## 4. 함수 정의
# 출력용 함수 임베딩
def get_embedding(text, model):
    client = OpenAI() # client가 논타입이라면 주석 해제.
    text = text.replace("\n", " ")
    return client.embeddings.create(input = [text], model=model).data[0].embedding
# 함수 구조 : output_parser 호출 / chain 초기화 / invoke 호출 / 리턴
# 사용자 의도 해석 함수
def interpret_user_intent(user_input):
    output_parser = StrOutputParser()
    print(f"DEBUG: Type : {type(template_user_intention)}/{type(llm)}/{type(output_parser)}", flush=True)
    chain = template_user_intention | llm | output_parser
    global history_user_intent
    history_user_intent = chain.invoke({"previous_memory" : history_user_memory, "user_input" : user_input})
    return history_user_intent
# NPC 의도 추론 함수
def interpret_npc_intent(user_input, user_intent, npc_features):
    output_parser = StrOutputParser()
    chain = template_npc_intention | llm | output_parser
    global history_npc_intent
    history_npc_intent = chain.invoke({"previous_memory" : history_user_memory, "user_input" : user_input, "user_intent" : user_intent, "npc_features" : npc_features})
    return history_npc_intent
# NPC 응답 생성 함수
def inference_answer_npc_answer(user_input, user_intent, npc_features, npc_intent):
    output_parser = StrOutputParser()
    chain = template_npc_answer | llm | output_parser
    global history_npc_answer
    history_npc_answer = chain.invoke({"previous_memory" : history_user_memory, "user_input" : user_input, "user_intent" : user_intent, "npc_features" : npc_features, "npc_intent" : npc_intent})
    return history_npc_answer
# 유사도 검사 함수
# 결과값은 구간 [-1, 1]에 있으며, -1에 가까울수록 반대되는 의미, 0일수록 관계없음, 1일수록 유사되는 의미를 갖고 있음
def vector_similarity(vector_a, vector_b):
    return numpy.dot(numpy.array(vector_a), numpy.array(vector_b)) / (numpy.linalg.norm(vector_a) * numpy.linalg.norm(vector_b))
# NPC 실행 함수 탐색 함수
def choose_npc_function(npc_intent):
    m_embedding_npc_intent = get_embedding(npc_intent, model_name_current_embedding)
    answer = -1;
    max = -100;
    for index in range(len(array_embedding_function_descriptions)):
        similarity = vector_similarity(m_embedding_npc_intent, array_embedding_function_descriptions[index])
        if max < similarity:
            max = similarity
            answer = index
    return answer
# NPC 유저를 향한 평판 함수

# NPC 기억 함수
def memory_previous(previous_memory, current_user_input, current_npc_output):
    output_parser = StrOutputParser()
    chain = template_memory_chat_summery | llm | output_parser
    global history_previous_summation
    history_previous_summation = chain.invoke({"previous_memory" : previous_memory, "user_input" : current_user_input, "npc_output" : current_npc_output})
    return history_previous_summation
## 5. 로직



def main():
    
    initialize()
    print(f"EVENT: READY", flush=True)
    
    while True:
        try:
            # 입력 받기
            # line = sys.stdin.readline()
            line = None
            if is_running_in_jupyter_notebook:
                line = input("Enter your input: ")
            else:
                
                line = sys.stdin.readline()
            
            if not line:
                continue
            print(f"Python received line.", flush=True)
            history_user_input = line
            # 3초 대기
            # time.sleep(3)
            print(f"DEBUG: [00] 유저 입력 {history_user_input}", flush=True)
            interpret_user_intent(history_user_input)
            print(f"DEBUG: [01] 유저 의도 {history_user_intent}", flush=True)
            interpret_npc_intent(history_user_input, history_user_intent, npc_features_goblin)
            print(f"DEBUG: [02] NPC 의도  {history_npc_intent}", flush=True)
            inference_answer_npc_answer(history_user_input, history_user_intent, npc_features_goblin, history_npc_intent)
            functionIndex = choose_npc_function(history_npc_intent)
            print(f"EVENT: receive start", flush=True)
            print(f"EVENT: {functionIndex}/{history_npc_answer}", flush=False)
            print(f"EVENT: receive end", flush=True)
            
            memory_previous(history_previous_summation, history_user_input, history_npc_answer)
            # print(f"DEBUG: {history_previous_summation}", flush=True)

        except Exception as e:
            print(f"Error: {e}", flush=True)

if __name__ == "__main__":
    main()
